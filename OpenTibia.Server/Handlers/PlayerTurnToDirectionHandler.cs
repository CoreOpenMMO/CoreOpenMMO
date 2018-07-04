// <copyright file="PlayerTurnToDirectionHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers
{
    using OpenTibia.Communications;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Notifications;

    internal class PlayerTurnToDirectionHandler : IncomingPacketHandler
    {
        public Direction Direction { get; }

        public PlayerTurnToDirectionHandler(Direction direction)
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

            player.TurnToDirection(this.Direction);

            Game.Instance.NotifySpectatingPlayers(conn => new CreatureTurnedNotification(conn, player), player.Location);
        }
    }
}