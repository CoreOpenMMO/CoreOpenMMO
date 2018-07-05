// <copyright file="LoadWorldHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using COMMO.Communications;
using COMMO.Communications.Interfaces;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Configuration;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Server.Handlers.Management
{
    internal class LoadWorldHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            // No incoming packet is required to load here.
            var gameConfig = ServiceConfiguration.GetConfiguration();

            ResponsePackets.Add(new WorldConfigPacket
            {
                WorldType = (byte)gameConfig.WorldType,
                DailyResetHour = gameConfig.DailyResetHour,
                IpAddressBytes = gameConfig.PrivateGameIpAddress.GetAddressBytes(),
                Port = gameConfig.PrivateGamePort,
                MaximumTotalPlayers = gameConfig.MaximumTotalPlayers,
                PremiumMainlandBuffer = gameConfig.PremiumMainlandBuffer,
                MaximumRookgardians = gameConfig.MaximumRookgardians,
                PremiumRookgardiansBuffer = gameConfig.PremiumRookgardiansBuffer
            });
        }
    }
}