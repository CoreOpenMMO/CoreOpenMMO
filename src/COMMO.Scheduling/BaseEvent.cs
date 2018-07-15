// <copyright file="BaseEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using COMMO.Scheduling.Contracts;
using Priority_Queue;

namespace COMMO.Scheduling
{
    /// <summary>
    /// Abstract class that represents the base event for scheduling.
    /// </summary>
    public abstract class BaseEvent : FastPriorityQueueNode, IEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEvent"/> class.
        /// </summary>
        /// <param name="evaluationTime">Optional. The time at which the event's conditions should be evaluated. Default is <see cref="EvaluationTime.OnExecute"/>.</param>
        public BaseEvent(EvaluationTime evaluationTime = EvaluationTime.OnExecute)
        {
            EventId = Guid.NewGuid().ToString("N");
            RequestorId = 0;
            EvaluateAt = evaluationTime;

            Conditions = new List<IEventCondition>();
            ActionsOnPass = new List<IEventAction>();
            ActionsOnFail = new List<IEventAction>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEvent"/> class.
        /// </summary>
        /// <param name="requestorId">Optional. The id of the creature or entity requesting the event. Default is 0.</param>
        /// <param name="evaluationTime">Optional. The time at which the event's conditions should be evaluated. Default is <see cref="EvaluationTime.OnExecute"/>.</param>
        public BaseEvent(uint requestorId = 0, EvaluationTime evaluationTime = EvaluationTime.OnExecute)
            : this(evaluationTime)
        {
            RequestorId = requestorId;
        }

        /// <inheritdoc/>
        public string EventId { get; }

        /// <inheritdoc/>
        public uint RequestorId { get; }

        /// <inheritdoc/>
        public string ErrorMessage { get; protected set; }

        /// <inheritdoc/>
        public EvaluationTime EvaluateAt { get; }

        public bool Force { get; protected set; }

        /// <inheritdoc/>
        public bool CanBeExecuted
        {
            get
            {
                var allPassed = true;

                if (!Force)
                {
                    foreach (var policy in Conditions)
                    {
                        allPassed &= policy.Evaluate();

                        if (!allPassed)
                        {
                            // TODO: proper logging.
                            Console.WriteLine($"Failed event condition {policy.GetType().Name}.");
                            ErrorMessage = policy.ErrorMessage;
                            break;
                        }
                    }
                }

                return allPassed;
            }
        }

        /// <inheritdoc/>
        public IList<IEventCondition> Conditions { get; }

        /// <inheritdoc/>
        public IList<IEventAction> ActionsOnPass { get; }

        /// <inheritdoc/>
        public IList<IEventAction> ActionsOnFail { get; }

        /// <summary>
        /// Executes the event. Performs the <see cref="ActionsOnPass"/> on the <see cref="ActionsOnFail"/> depending if the conditions were met.
        /// </summary>
        public void Process()
        {
            if (EvaluateAt == EvaluationTime.OnSchedule || CanBeExecuted)
            {
                int i = 1;
                foreach (var action in ActionsOnPass)
                {
                    var sw = Stopwatch.StartNew();
                    action.Execute();
                    sw.Stop();

                    Console.WriteLine($"Executed ({i++} of {ActionsOnPass.Count})... done in {sw.Elapsed}.");
                }

                return;
            }

            foreach (var action in ActionsOnFail)
            {
                action.Execute();
            }
        }
    }
}