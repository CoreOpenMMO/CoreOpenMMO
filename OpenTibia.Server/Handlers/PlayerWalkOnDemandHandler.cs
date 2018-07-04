// <copyright file="PlayerWalkOnDemandHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers
{
    using System;
    using System.Threading.Tasks;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data;

    internal class PlayerWalkOnDemandHandler : IncomingPacketHandler
    {
        public Direction Direction { get; }

        public PlayerWalkOnDemandHandler(Direction direction)
        {
            this.Direction = direction;
        }

        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            // No other content in message.
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            if (player == null)
            {
                return;
            }

            player.ClearPendingActions();

            var cooldownRemaining = player.CalculateRemainingCooldownTime(CooldownType.Move, DateTime.Now);

            if (!Game.Instance.RequestCreatureWalkToDirection(player, this.Direction, cooldownRemaining))
            {
                this.ResponsePackets.Add(new PlayerWalkCancelPacket
                {
                    Direction = player.Direction
                });

                this.ResponsePackets.Add(new TextMessagePacket
                {
                    Message = "Sorry, not possible.",
                    Type = MessageType.StatusSmall
                });
            }
        }
    }
}