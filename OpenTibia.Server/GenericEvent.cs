// <copyright file="GenericEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server
{
    using OpenTibia.Scheduling;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Interfaces;

    /// <summary>
    /// Class that represents a generic event.
    /// </summary>
    internal class GenericEvent : BaseEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericEvent"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting this event.</param>
        /// <param name="conditions">The conditions of the event.</param>
        /// <param name="onSuccessActions">The actions to run if the event passes conditions.</param>
        /// <param name="onFailActions">The actions to run if the event fails to pass conditions.</param>
        /// <param name="evaluationTime">Optional. The time on which the event's conditions should be evaluated. Default is <see cref="EvaluationTime.OnBoth"/>.</param>
        public GenericEvent(uint requestorId, IEventCondition [] conditions, IEventAction [] onSuccessActions, IEventAction[] onFailActions, EvaluationTime evaluationTime = EvaluationTime.OnBoth)
            : base(requestorId, evaluationTime)
        {
            if (conditions != null)
            {
                foreach (var condition in conditions)
                {
                    this.Conditions.Add(condition);
                }
            }

            if (onSuccessActions != null)
            {
                foreach (var action in onSuccessActions)
                {
                    this.ActionsOnPass.Add(action);
                }
            }

            if (onFailActions != null)
            {
                foreach (var action in onFailActions)
                {
                    this.ActionsOnFail.Add(action);
                }
            }
        }

        /// <summary>
        /// Gets the creature that is requesting the event, if 
        /// </summary>
        public ICreature Requestor
        {
            get
            {
                return this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);
            }
        }
    }
}