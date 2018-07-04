// <copyright file="IEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Scheduling.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface that represents an event.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Gets a <see cref="EvaluationTime"/> value indicating when this event should be evaluated.
        /// </summary>
        EvaluationTime EvaluateAt { get; }

        /// <summary>
        /// Gets a unique identifier for this event.
        /// </summary>
        string EventId { get; }

        /// <summary>
        /// Gets the id of the requestor of this event, if available.
        /// </summary>
        uint RequestorId { get; }

        /// <summary>
        /// Gets a value indicating whether the event can be executed.
        /// </summary>
        bool CanBeExecuted { get; }

        /// <summary>
        /// Gets the error message that should be bubbled back to the player if the event cannot be executed.
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// Gets the collection of conditional <see cref="IEventCondition"/> that the event must pass on evaluation.
        /// </summary>
        IList<IEventCondition> Conditions { get; }

        /// <summary>
        /// Gets the collection of <see cref="IEventAction"/> that will be executed if the conditions check succeeds.
        /// </summary>
        IList<IEventAction> ActionsOnPass { get; }

        /// <summary>
        /// Gets the collection of <see cref="IEventAction"/> that will be executed if the conditions check fails.
        /// </summary>
        IList<IEventAction> ActionsOnFail { get; }
        
        /// <summary>
        /// Processes the event.
        /// </summary>
        void Process();
    }
}