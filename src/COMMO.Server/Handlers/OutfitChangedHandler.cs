// <copyright file="OutfitChangedHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Communications;
using COMMO.Communications.Packets.Incoming;
using COMMO.Server.Data;
using COMMO.Server.Notifications;

namespace COMMO.Server.Handlers
{
    internal class OutfitChangedHandler : IncomingPacketHandler
    {
        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var packet = new OutfitChangedPacket(message);
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            if (player == null)
            {
                return;
            }

            // TODO: check if player actually has permissions to change outfit.
            player.SetOutfit(packet.Outfit);

            Game.Instance.NotifySpectatingPlayers(conn => new CreatureChangedOutfitNotification(conn, player), player.Location);
        }
    }
}