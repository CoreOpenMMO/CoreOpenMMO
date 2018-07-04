// <copyright file="ItemMoveHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers
{
    using System.Linq;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Packets.Incoming;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Actions;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Models.Structs;

    internal class ItemMoveHandler : IncomingPacketHandler
    {
        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var itemMovePacket = new ItemMovePacket(message);
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            if (player == null)
            {
                return;
            }

            player.ClearPendingActions();

            // Before actually moving the item, check if we're close enough to use it.
            if (itemMovePacket.FromLocation.Type == LocationType.Ground)
            {
                var locationDiff = itemMovePacket.FromLocation - player.Location;

                if (locationDiff.Z != 0) // it's on a different floor...
                {
                    this.ResponsePackets.Add(new TextMessagePacket
                    {
                        Type = MessageType.StatusSmall,
                        Message = "There is no way."
                    });

                    return;
                }

                if (locationDiff.MaxValueIn2D > 1)
                {
                    // Too far away to use it.
                    Location retryLoc;
                    var directions = Game.Instance.Pathfind(player.Location, itemMovePacket.FromLocation, out retryLoc).ToArray();

                    player.SetPendingAction(new MoveItemPlayerAction(player, itemMovePacket, retryLoc));

                    if (directions.Length > 0)
                    {
                        player.AutoWalk(directions);
                    }
                    else // we found no way...
                    {
                        this.ResponsePackets.Add(new TextMessagePacket
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