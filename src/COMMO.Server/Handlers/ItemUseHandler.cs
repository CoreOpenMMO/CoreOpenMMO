// <copyright file="ItemUseHandler.cs" company="2Dudes">
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
    internal class ItemUseHandler : IncomingPacketHandler
    {
        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var itemUsePacket = new ItemUsePacket(message);

			if (!(Game.Instance.GetCreatureWithId(connection.PlayerId) is Player player)) {
				return;
			}

			player.ClearPendingActions();

            // Before actually using the item, check if we're close enough to use it.
            if (itemUsePacket.FromLocation.Type == LocationType.Ground)
            {
                var locationDiff = itemUsePacket.FromLocation - player.Location;

                if (locationDiff.Z != 0)
                {
                    // it's on a different floor...
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
					var directions = Game.Instance.Pathfind(player.Location, itemUsePacket.FromLocation, out var retryLoc).ToArray();

					player.SetPendingAction(new UseItemPlayerAction(player, itemUsePacket, retryLoc));

                    if (directions.Any())
                    {
                        player.AutoWalk(directions);
                    }
                    else
                    {
                        // we found no way...
                        ResponsePackets.Add(new TextMessagePacket
                        {
                            Type = MessageType.StatusSmall,
                            Message = "There is no way."
                        });
                    }

                    return;
                }
            }

            new UseItemPlayerAction(player, itemUsePacket, itemUsePacket.FromLocation).Perform();
        }
    }
}