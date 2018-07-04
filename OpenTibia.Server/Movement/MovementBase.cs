// <copyright file="MovementBase.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement
{
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Scheduling;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Notifications;

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
            this.ActionsOnFail.Add(
                new GenericEventAction(
                    () =>
                    {
                        var player = this.Requestor as Player;

                        if (player != null)
                        {
                            Game.Instance.NotifySinglePlayer(player, conn =>
                                new GenericNotification(
                                    conn,
                                    new PlayerWalkCancelPacket { Direction = player.ClientSafeDirection },
                                    new TextMessagePacket { Message = this.ErrorMessage ?? "Sorry, not possible.", Type = MessageType.StatusSmall }));
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
                if (this.requestor == null)
                {
                    this.requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);
                }

                return this.requestor;
            }
        }
    }
}
