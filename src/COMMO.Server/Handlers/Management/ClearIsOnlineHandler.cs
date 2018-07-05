// <copyright file="ClearIsOnlineHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Handlers.Management
{
    using System.Collections.Generic;
    using System.Linq;
    using COMMO.Communications;
    using COMMO.Communications.Interfaces;
    using COMMO.Communications.Packets.Incoming;
    using COMMO.Communications.Packets.Outgoing;
    using COMMO.Data;
    using COMMO.Server.Data;
    using COMMO.Server.Data.Interfaces;

    internal class ClearIsOnlineHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var clearOnlinePacket = new DefaultReadPacket(message);

            using (var otContext = new OpenTibiaDbContext())
            {
                var onlinePlayers = otContext.Players.Where(p => p.Online > 0).ToList();

                foreach (var player in onlinePlayers)
                {
                    player.Online = 0;
                }

                otContext.SaveChanges();

                ResponsePackets.Add(new ClearOnlinePlayersResultPacket
                {
                    ClearedCount = (ushort)onlinePlayers.Count
                });
            }
        }
    }
}