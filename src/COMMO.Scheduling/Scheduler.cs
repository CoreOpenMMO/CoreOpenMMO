// <copyright file="Scheduler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using COMMO.Common.Helpers;
using COMMO.Scheduling.Contracts;
using Priority_Queue;

namespace COMMO.Scheduling
{
    /// <summary>
    /// Class that represents a scheduler for events.
    /// </summary>
    public class Scheduler : IScheduler
    {
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
        private static readonly TimeSpan DefaultProcessWaitTime = TimeSpan.FromMinutes(1);

        /// <summary>
        /// The referenced start time.
        /// </summary>
        private readonly DateTime startTime;

        /// <summary>
        /// The internal priority queue used to manage events.
        /// </summary>
        private FastPriorityQueue<BaseEvent> priorityQueue;

        /// <summary>
        /// Stores the ids of cancelled events.
        /// </summary>
        private ISet<string> cancelledEvents;

        /// <summary>
        /// A dictionary to keep track of who requested which events.
        /// </summary>
        private IDictionary<uint, ISet<string>> eventsPerRequestor;

        /// <summary>
        /// A cancellation token to use on the queue processing thread.
        /// </summary>
        private CancellationToken cancellationToken;

        /// <summary>
        /// A lock object to semaphore queue modifications.
        /// </summary>
        private object queueLock;

        /// <summary>
        /// A lock object to monitor when new events are added to the queue.
        /// </summary>
        private object eventsAvailableLock;

        /// <summary>
        /// A lock object to semaphore the events per requestor dictionary.
        /// </summary>
        private object eventsPerRequestorLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        /// <param name="referenceTime">The time to use as reference .</param>
        public Scheduler(DateTime referenceTime)
        {
            referenceTime.ThrowIfDefaultValue(nameof(referenceTime));

            var refTimeDifference = (DateTime.Now - referenceTime).TotalHours;

            if (refTimeDifference >= Scheduler.MaximumReferenceTimeDifferenceInHours)
            {
                throw new ArgumentException($"{nameof(referenceTime)} must be within the past hour's time.");
            }

            eventsPerRequestorLock = new object();
            eventsAvailableLock = new object();
            queueLock = new object();
            startTime = referenceTime;
            priorityQueue = new FastPriorityQueue<BaseEvent>(Scheduler.MaxQueueNodes);
            cancellationToken = new CancellationTokenSource().Token;
            cancelledEvents = new HashSet<string>();
            eventsPerRequestor = new Dictionary<uint, ISet<string>>();

            Task.Factory.StartNew(QueueProcessing, cancellationToken);
        }

        /// <inheritdoc/>
        public event EventFired OnEventFired;

        /// <summary>
        /// Cancels all the events attributed to a requestor.
        /// </summary>
        /// <param name="requestorId">The id of the requestor.</param>
        /// <param name="specificType">Optional. The type of event to remove. By default, it will remove all.</param>
        public void CancelAllFor(uint requestorId, Type specificType = null)
        {
            requestorId.ThrowIfDefaultValue();

            if (specificType == null)
            {
                specificType = typeof(BaseEvent);
            }

            if (specificType as IEvent == null)
            {
                throw new ArgumentException($"Invalid type of event specified. Type must derive from {nameof(IEvent)}.", nameof(specificType));
            }

            lock (eventsPerRequestorLock)
            {
                if (!eventsPerRequestor.ContainsKey(requestorId) || eventsPerRequestor[requestorId].Count == 0)
                {
                    return;
                }

                foreach (var eventId in eventsPerRequestor[requestorId])
                {
                    // TODO: remove only the specified type.
                    CancelEvent(eventId);
                }

                eventsPerRequestor.Remove(requestorId);
            }
        }

        /// <summary>
        /// Cancels an event.
        /// </summary>
        /// <param name="eventId">The id of the event to cancel.</param>
        public void CancelEvent(string eventId)
        {
            eventId.ThrowIfNullOrWhiteSpace();

            try
            {
                cancelledEvents.Add(eventId);
            }
            catch
            {
                // just ignore any collisions.
            }
        }

        /// <inheritdoc/>
        public void ImmediateEvent(IEvent eventToSchedule)
        {
            eventToSchedule.ThrowIfNull(nameof(eventToSchedule));

            var castedEvent = eventToSchedule as BaseEvent;

            if (castedEvent == null)
            {
                throw new ArgumentException($"Argument must be of type {nameof(BaseEvent)}.", nameof(eventToSchedule));
            }

            lock (eventsAvailableLock)
            {
                lock (queueLock)
                {
                    if (priorityQueue.Contains(castedEvent))
                    {
                        throw new ArgumentException($"The event is already scheduled.", nameof(eventToSchedule));
                    }

                    priorityQueue.Enqueue(castedEvent, 0);
                }

                // check and add event attribution to the requestor
                if (castedEvent.RequestorId > 0)
                {
                    lock (eventsPerRequestorLock)
                    {
                        if (!eventsPerRequestor.ContainsKey(castedEvent.RequestorId))
                        {
                            eventsPerRequestor.Add(castedEvent.RequestorId, new HashSet<string>());
                        }

                        try
                        {
                            eventsPerRequestor[castedEvent.RequestorId].Add(castedEvent.EventId);
                        }
                        catch
                        {
                            // ignore clashes, it was our goal to add it anyways.
                        }
                    }
                }

                Monitor.Pulse(eventsAvailableLock);
            }
        }

        /// <inheritdoc/>
        public void ScheduleEvent(IEvent eventToSchedule, DateTime runAt)
        {
            eventToSchedule.ThrowIfNull(nameof(eventToSchedule));

            var castedEvent = eventToSchedule as BaseEvent;

            if (castedEvent == null)
            {
                throw new ArgumentException($"Argument must be of type {nameof(BaseEvent)}.", nameof(eventToSchedule));
            }

            runAt.ThrowIfDefaultValue(nameof(runAt));

            if (runAt < startTime)
            {
                throw new ArgumentException($"Value cannot be earlier than the reference time of the scheduler: {startTime}.", nameof(runAt));
            }

            lock (eventsAvailableLock)
            {
                lock (queueLock)
                {
                    if (priorityQueue.Contains(castedEvent))
                    {
                        throw new ArgumentException($"The event is already scheduled.", nameof(eventToSchedule));
                    }

                    priorityQueue.Enqueue(castedEvent, GetMillisecondsAfterReferenceTime(runAt));
                }

                // check and add event attribution to the requestor
                if (castedEvent.RequestorId > 0)
                {
                    lock (eventsPerRequestorLock)
                    {
                        if (!eventsPerRequestor.ContainsKey(castedEvent.RequestorId))
                        {
                            eventsPerRequestor.Add(castedEvent.RequestorId, new HashSet<string>());
                        }

                        try
                        {
                            eventsPerRequestor[castedEvent.RequestorId].Add(castedEvent.EventId);
                        }
                        catch
                        {
                            // ignore clashes, it was our goal to add it anyways.
                        }
                    }
                }

                Monitor.Pulse(eventsAvailableLock);
            }
        }

        /// <summary>
        /// Calculates the total millisenconds value of the time difference between the specified time and when the scheduler began.
        /// </summary>
        /// <param name="dateTime">The specified time.</param>
        /// <returns>The milliseconds value.</returns>
        private long GetMillisecondsAfterReferenceTime(DateTime dateTime)
        {
            return Convert.ToInt64((dateTime - startTime).TotalMilliseconds);
        }

        /// <summary>
        /// Processes the queue and fires events.
        /// </summary>
        private void QueueProcessing()
        {
            TimeSpan waitForNewTimeOut = TimeSpan.Zero;

            while (!cancellationToken.IsCancellationRequested)
            {
                lock (eventsAvailableLock)
                {
                    // wait until we're flagged that there are events available.
                    Monitor.Wait(eventsAvailableLock, waitForNewTimeOut > TimeSpan.Zero ? waitForNewTimeOut : Scheduler.DefaultProcessWaitTime);

                    // reset time to wait.
                    waitForNewTimeOut = TimeSpan.Zero;

                    lock (queueLock)
                    {
                        if (priorityQueue.First == null)
                        {
                            // no more items on the queue, go to wait.
                            Console.WriteLine($"{nameof(Scheduler)}: Queue empty.");
                            continue;
                        }

                        // store a single 'current' time for processing of all items in the queue
                        // TODO: use 'current' time from Game.Instance
                        var currentTimeInMilliseconds = GetMillisecondsAfterReferenceTime(DateTime.Now);

                        // check the current queue and fire any events that are due.
                        while (priorityQueue.Count > 0)
                        {
                            // the first item always points to the next-in-time event available.
                            var nextEvent = priorityQueue.First;

                            // check if this event has been cancelled.
                            if (cancelledEvents.Contains(nextEvent.EventId))
                            {
                                // dequeue, clean and move next.
                                priorityQueue.Dequeue();
                                CleanAllAttributedTo(nextEvent.EventId, nextEvent.RequestorId);
                                continue;
                            }

                            // check if the event is due
                            if (nextEvent.Priority <= currentTimeInMilliseconds)
                            {
                                // actually dequeue this item.
                                priorityQueue.Dequeue();

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
        private void CleanAllAttributedTo(string eventId, uint eventRequestor = 0)
        {
            try
            {
                cancelledEvents.Remove(eventId);
            }
            catch
            {
                // ignore, as if this fails then the value is not there.
            }

            if (eventRequestor == 0)
            {
                // no requestor, so it shouldn't be on the other dictionary.
                return;
            }

            try
            {
                lock (eventsPerRequestorLock)
                {
                    eventsPerRequestor[eventRequestor].Remove(eventId);

                    if (eventsPerRequestor[eventRequestor].Count == 0)
                    {
                        eventsPerRequestor.Remove(eventRequestor);
                    }
                }
            }
            catch
            {
                // ignore, as if this fails then the value is not there.
            }
        }
    }
}
