// <copyright file="PlayerTurnToDirectionHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Communications;
using COMMO.Data.Contracts;
using COMMO.Server.Data;
using COMMO.Server.Notifications;

namespace COMMO.Server.Handlers
{
    internal class PlayerTurnToDirectionHandler : IncomingPacketHandler
    {
        public Direction Direction { get; }

        public PlayerTurnToDirectionHandler(Direction direction)
        {
            Direction = direction;
        }

        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
			// No other content in message.

			if (!(Game.Instance.GetCreatureWithId(connection.PlayerId) is Player player)) {
				return;
			}

			player.TurnToDirection(Direction);

            Game.Instance.NotifySpectatingPlayers(conn => new CreatureTurnedNotification(conn, player), player.Location);
        }
    }
}