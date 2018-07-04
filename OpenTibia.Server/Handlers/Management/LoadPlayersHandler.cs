// <copyright file="LoadPlayersHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers.Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Interfaces;
    using OpenTibia.Communications.Packets.Incoming;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    internal class LoadPlayersHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var loadPlayersPacket = new DefaultReadPacket(message);

            using (var otContext = new OpenTibiaDbContext())
            {
                var thirtyDaysBack = DateTime.Today.AddDays(-30).ToFileTimeUtc();

                var loadedPlayers = otContext.Players.Where(p => p.Lastlogin > thirtyDaysBack);

                this.ResponsePackets.Add(new LoadPlayersResultPacket
                {
                    LoadedPlayers = loadedPlayers.ToList()
                });
            }
        }
    }
}