// <copyright file="ClearIsOnlineHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers.Management
{
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Interfaces;
    using OpenTibia.Communications.Packets.Incoming;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

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

                this.ResponsePackets.Add(new ClearOnlinePlayersResultPacket
                {
                    ClearedCount = (ushort)onlinePlayers.Count
                });
            }
        }
    }
}