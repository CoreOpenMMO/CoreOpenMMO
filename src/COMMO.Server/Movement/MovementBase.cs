// <copyright file="MovementBase.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Communications.Packets.Outgoing;
using COMMO.Data.Contracts;
using COMMO.Scheduling;
using COMMO.Scheduling.Contracts;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Notifications;

namespace COMMO.Server.Movement
{
    /// <summary>
    /// Class that represents a common base bewteen movements.
    /// </summary>
    internal abstract class MovementBase : BaseEvent
    {
        /// <summary>
        /// Caches the requestor creature, if defined.
        /// </summary>
        private ICreature requestor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MovementBase"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the movement.</param>
        /// <param name="evaluationTime">The time to evaluate the movement.</param>
        protected MovementBase(uint requestorId, EvaluationTime evaluationTime)
            : base(requestorId, evaluationTime)
        {
            ActionsOnFail.Add(
                new GenericEventAction(
                    () =>
                    {
						if (Requestor is Player player) {
							Game.Instance.NotifySinglePlayer(player, conn =>
								new GenericNotification(
									conn,
									new PlayerWalkCancelPacket { Direction = player.ClientSafeDirection },
									new TextMessagePacket { Message = ErrorMessage ?? "Sorry, not possible.", Type = MessageType.StatusSmall }));
						}
					}));
        }

        /// <summary>
        /// Gets the creature that is requesting the event, if known.
        /// </summary>
        public ICreature Requestor
        {
            get
            {
                if (requestor == null)
                {
                    requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);
                }

                return requestor;
            }
        }
    }
}
