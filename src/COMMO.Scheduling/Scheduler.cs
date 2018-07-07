// <copyright file="Scheduler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Scheduling.Contracts;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace COMMO.Scheduling {
	/// <summary>
	/// Class that represents a scheduler for events.
	/// </summary>
	public class Scheduler : IScheduler {
		/// <summary>
		/// The maximum number of nodes that the internal queue can hold.
		/// </summary>
		/// <remarks>Arbitrarily chosen, resize as needed.</remarks>
		private const int MaxQueueNodes = 1000000;

		/// <summary>
		/// The maximum difference in hours that the referenced time can be off on creaation of the <see cref="Scheduler"/> instance.
		/// </summary>
		private const int MaximumReferenceTimeDifferenceInHours = 1;

		/// <summary>
		/// The default processing wait time on the processing queue thread.
		/// </summary>
		private static readonly TimeSpan _defaultProcessWaitTime = TimeSpan.FromMinutes(1);

		/// <summary>
		/// The referenced start time.
		/// </summary>
		private readonly DateTime _startTime;

		/// <summary>
		/// The internal priority queue used to manage events.
		/// </summary>
		private FastPriorityQueue<BaseEvent> _priorityQueue;

		/// <summary>
		/// Stores the ids of cancelled events.
		/// </summary>
		private ISet<string> _cancelledEvents;

		/// <summary>
		/// A dictionary to keep track of who requested which events.
		/// </summary>
		private IDictionary<uint, ISet<string>> _eventsPerRequestor;

		/// <summary>
		/// A cancellation token to use on the queue processing thread.
		/// </summary>
		private CancellationToken _cancellationToken;

		/// <summary>
		/// A lock object to semaphore queue modifications.
		/// </summary>
		private readonly object _queueLock;

		/// <summary>
		/// A lock object to monitor when new events are added to the queue.
		/// </summary>
		private readonly object _eventsAvailableLock;

		/// <summary>
		/// A lock object to semaphore the events per requestor dictionary.
		/// </summary>
		private readonly object _eventsPerRequestorLock;

		/// <summary>
		/// Initializes a new instance of the <see cref="Scheduler"/> class.
		/// </summary>
		/// <param name="referenceTime">The time to use as reference .</param>
		public Scheduler(DateTime referenceTime) {

			var refTimeDifference = (DateTime.Now - referenceTime).TotalHours;

			if (refTimeDifference >= Scheduler.MaximumReferenceTimeDifferenceInHours) {
				throw new ArgumentException($"{nameof(referenceTime)} must be within the past hour's time.");
			}

			_eventsPerRequestorLock = new object();
			_eventsAvailableLock = new object();
			_queueLock = new object();
			_startTime = referenceTime;
			_priorityQueue = new FastPriorityQueue<BaseEvent>(Scheduler.MaxQueueNodes);
			_cancellationToken = new CancellationTokenSource().Token;
			_cancelledEvents = new HashSet<string>();
			_eventsPerRequestor = new Dictionary<uint, ISet<string>>();

			Task.Factory.StartNew(QueueProcessing, _cancellationToken);
		}

		/// <inheritdoc/>
		public event EventFired OnEventFired;

		/// <summary>
		/// Cancels all the events attributed to a requestor.
		/// </summary>
		/// <param name="requestorId">The id of the requestor.</param>
		/// <param name="specificType">Optional. The type of event to remove. By default, it will remove all.</param>
		public void CancelAllFor(uint requestorId, Type specificType = null) {
			if (specificType == null) {
				specificType = typeof(BaseEvent);
			}

			if (specificType as IEvent == null) {
				throw new ArgumentException($"Invalid type of event specified. Type must derive from {nameof(IEvent)}.", nameof(specificType));
			}

			lock (_eventsPerRequestorLock) {
				if (!_eventsPerRequestor.ContainsKey(requestorId) || _eventsPerRequestor[requestorId].Count == 0) {
					return;
				}

				foreach (var eventId in _eventsPerRequestor[requestorId]) {
					// TODO: remove only the specified type.
					CancelEvent(eventId);
				}

				_eventsPerRequestor.Remove(requestorId);
			}
		}

		/// <summary>
		/// Cancels an event.
		/// </summary>
		/// <param name="eventId">The id of the event to cancel.</param>
		public void CancelEvent(string eventId) {
			if (string.IsNullOrWhiteSpace(eventId))
				throw new ArgumentException(nameof(eventId) + "can't be null or whitespace.");

			try {
				_cancelledEvents.Add(eventId);
			} catch {
				// just ignore any collisions.
			}
		}

		/// <inheritdoc/>
		public void ImmediateEvent(IEvent eventToSchedule) {
			if (eventToSchedule == null)
				throw new ArgumentNullException(nameof(eventToSchedule));

			if (!(eventToSchedule is BaseEvent castedEvent)) {
				throw new ArgumentException($"Argument must be of type {nameof(BaseEvent)}.", nameof(eventToSchedule));
			}

			lock (_eventsAvailableLock) {
				lock (_queueLock) {
					if (_priorityQueue.Contains(castedEvent)) {
						throw new ArgumentException($"The event is already scheduled.", nameof(eventToSchedule));
					}

					_priorityQueue.Enqueue(castedEvent, 0);
				}

				// check and add event attribution to the requestor
				if (castedEvent.RequestorId > 0) {
					lock (_eventsPerRequestorLock) {
						if (!_eventsPerRequestor.ContainsKey(castedEvent.RequestorId)) {
							_eventsPerRequestor.Add(castedEvent.RequestorId, new HashSet<string>());
						}

						try {
							_eventsPerRequestor[castedEvent.RequestorId].Add(castedEvent.EventId);
						} catch {
							// ignore clashes, it was our goal to add it anyways.
						}
					}
				}

				Monitor.Pulse(_eventsAvailableLock);
			}
		}

		/// <inheritdoc/>
		public void ScheduleEvent(IEvent eventToSchedule, DateTime runAt) {
			if (eventToSchedule == null)
				throw new ArgumentNullException(nameof(eventToSchedule));

			if (!(eventToSchedule is BaseEvent castedEvent)) {
				throw new ArgumentException($"Argument must be of type {nameof(BaseEvent)}.", nameof(eventToSchedule));
			}

			if (runAt < _startTime) {
				throw new ArgumentException($"Value cannot be earlier than the reference time of the scheduler: {_startTime}.", nameof(runAt));
			}

			lock (_eventsAvailableLock) {
				lock (_queueLock) {
					if (_priorityQueue.Contains(castedEvent)) {
						throw new ArgumentException($"The event is already scheduled.", nameof(eventToSchedule));
					}

					_priorityQueue.Enqueue(castedEvent, GetMillisecondsAfterReferenceTime(runAt));
				}

				// check and add event attribution to the requestor
				if (castedEvent.RequestorId > 0) {
					lock (_eventsPerRequestorLock) {
						if (!_eventsPerRequestor.ContainsKey(castedEvent.RequestorId)) {
							_eventsPerRequestor.Add(castedEvent.RequestorId, new HashSet<string>());
						}

						try {
							_eventsPerRequestor[castedEvent.RequestorId].Add(castedEvent.EventId);
						} catch {
							// ignore clashes, it was our goal to add it anyways.
						}
					}
				}

				Monitor.Pulse(_eventsAvailableLock);
			}
		}

		/// <summary>
		/// Calculates the total millisenconds value of the time difference between the specified time and when the scheduler began.
		/// </summary>
		/// <param name="dateTime">The specified time.</param>
		/// <returns>The milliseconds value.</returns>
		private long GetMillisecondsAfterReferenceTime(DateTime dateTime) {
			return Convert.ToInt64((dateTime - _startTime).TotalMilliseconds);
		}

		/// <summary>
		/// Processes the queue and fires events.
		/// </summary>
		private void QueueProcessing() {
			TimeSpan waitForNewTimeOut = TimeSpan.Zero;

			while (!_cancellationToken.IsCancellationRequested) {
				lock (_eventsAvailableLock) {
					// wait until we're flagged that there are events available.
					Monitor.Wait(_eventsAvailableLock, waitForNewTimeOut > TimeSpan.Zero ? waitForNewTimeOut : Scheduler._defaultProcessWaitTime);

					// reset time to wait.
					waitForNewTimeOut = TimeSpan.Zero;

					lock (_queueLock) {
						if (_priorityQueue.First == null) {
							// no more items on the queue, go to wait.
							Console.WriteLine($"{nameof(Scheduler)}: Queue empty.");
							continue;
						}

						// store a single 'current' time for processing of all items in the queue
						// TODO: use 'current' time from Game.Instance
						var currentTimeInMilliseconds = GetMillisecondsAfterReferenceTime(DateTime.Now);

						// check the current queue and fire any events that are due.
						while (_priorityQueue.Count > 0) {
							// the first item always points to the next-in-time event available.
							var nextEvent = _priorityQueue.First;

							// check if this event has been cancelled.
							if (_cancelledEvents.Contains(nextEvent.EventId)) {
								// dequeue, clean and move next.
								_priorityQueue.Dequeue();
								CleanAllAttributedTo(nextEvent.EventId, nextEvent.RequestorId);
								continue;
							}

							// check if the event is due
							if (nextEvent.Priority <= currentTimeInMilliseconds) {
								// actually dequeue this item.
								_priorityQueue.Dequeue();

								OnEventFired?.Invoke(this, new EventFiredEventArgs(nextEvent));
								continue;
							}

							// else the next item is in the future, so figure out how long to wait, update and break.
							waitForNewTimeOut = TimeSpan.FromMilliseconds(nextEvent.Priority < currentTimeInMilliseconds ? 0 : nextEvent.Priority - currentTimeInMilliseconds);
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Cleans all events attributed to the specified event or requestor id.
		/// </summary>
		/// <param name="eventId">The id of the event to cancel.</param>
		/// <param name="eventRequestor">The id of the requestor.</param>
		private void CleanAllAttributedTo(string eventId, uint eventRequestor = 0) {
			try {
				_cancelledEvents.Remove(eventId);
			} catch {
				// ignore, as if this fails then the value is not there.
			}

			if (eventRequestor == 0) {
				// no requestor, so it shouldn't be on the other dictionary.
				return;
			}

			try {
				lock (_eventsPerRequestorLock) {
					_eventsPerRequestor[eventRequestor].Remove(eventId);

					if (_eventsPerRequestor[eventRequestor].Count == 0) {
						_eventsPerRequestor.Remove(eventRequestor);
					}
				}
			} catch {
				// ignore, as if this fails then the value is not there.
			}
		}
	}
}
