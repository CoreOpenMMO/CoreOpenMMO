// <copyright file="ItemMoveHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;
using COMMO.Communications;
using COMMO.Communications.Packets.Incoming;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Data.Contracts;
using COMMO.Server.Actions;
using COMMO.Server.Data;
using COMMO.Server.Data.Models.Structs;

namespace COMMO.Server.Handlers
{
    internal class ItemMoveHandler : IncomingPacketHandler
    {
        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var itemMovePacket = new ItemMovePacket(message);

			if (!(Game.Instance.GetCreatureWithId(connection.PlayerId) is Player player)) {
				return;
			}

			player.ClearPendingActions();

            // Before actually moving the item, check if we're close enough to use it.
            if (itemMovePacket.FromLocation.Type == LocationType.Ground)
            {
                var locationDiff = itemMovePacket.FromLocation - player.Location;

                if (locationDiff.Z != 0) // it's on a different floor...
                {
                    ResponsePackets.Add(new TextMessagePacket
                    {
                        Type = MessageType.StatusSmall,
                        Message = "There is no way."
                    });

                    return;
                }

                if (locationDiff.MaxValueIn2D > 1)
                {
					// Too far away to use it.
					var directions = Game.Instance.Pathfind(player.Location, itemMovePacket.FromLocation, out var retryLoc).ToArray();

					player.SetPendingAction(new MoveItemPlayerAction(player, itemMovePacket, retryLoc));

                    if (directions.Length > 0)
                    {
                        player.AutoWalk(directions);
                    }
                    else // we found no way...
                    {
                        ResponsePackets.Add(new TextMessagePacket
                        {
                            Type = MessageType.StatusSmall,
                            Message = "There is no way."
                        });
                    }

                    return;
                }
            }

            new MoveItemPlayerAction(player, itemMovePacket, itemMovePacket.FromLocation).Perform();
        }
    }
}