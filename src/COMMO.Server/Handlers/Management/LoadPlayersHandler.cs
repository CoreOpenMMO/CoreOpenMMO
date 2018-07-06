// <copyright file="LoadPlayersHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using COMMO.Communications;
using COMMO.Communications.Interfaces;
using COMMO.Communications.Packets.Incoming;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Data;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Server.Handlers.Management
{
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

                ResponsePackets.Add(new LoadPlayersResultPacket
                {
                    LoadedPlayers = loadedPlayers.ToList()
                });
            }
        }
    }
}