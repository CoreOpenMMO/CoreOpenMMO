// <copyright file="IScheduler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Scheduling.Contracts
{
    using System;

    /// <summary>
    /// Represents an event being fired.
    /// </summary>
    /// <param name="sender">The sender of the event fired event.</param>
    /// <param name="eventArgs">The event arguments that contain the actual event that was fired.</param>
    public delegate void EventFired(object sender, EventFiredEventArgs eventArgs);

    /// <summary>
    /// Interface that represents a scheduler.
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// Event fired when an event gets fired by the scheduler.
        /// </summary>
        event EventFired OnEventFired;

        /// <summary>
        /// Schedules an event to be fired immediately.
        /// </summary>
        /// <param name="eventToSchedule">The event to schedule.</param>
        void ImmediateEvent(IEvent eventToSchedule);

        /// <summary>
        /// Schedules an event to be fired at the specified time.
        /// </summary>
        /// <param name="eventToSchedule">The event to schedule.</param>
        /// <param name="runAt">The time at which the event should be fired.</param>
        void ScheduleEvent(IEvent eventToSchedule, DateTime runAt);

        /// <summary>
        /// Cancels a specific event by it's id.
        /// </summary>
        /// <param name="eventId">The event to cancel.</param>
        void CancelEvent(string eventId);

        /// <summary>
        /// Cancels all events attributed to the specified requestor.
        /// </summary>
        /// <param name="requestorId">The id of the requestor.</param>
        /// <param name="specificType">Optional. The type of event to remove. By default, it will remove all.</param>
        void CancelAllFor(uint requestorId, Type specificType = null);
    }
}