// <copyright file="CharacterDeathHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers.Management
{
    using System.Collections.Generic;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Interfaces;
    using OpenTibia.Communications.Packets.Incoming;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data;
    using OpenTibia.Data.Models;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    internal class CharacterDeathHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var characterDeathPacket = new CharacterDeathPacket(message);

            using (var otContext = new OpenTibiaDbContext())
            {
                var playerKilledPlayer = characterDeathPacket.KillerId > 0;

                otContext.Deaths.Add(new Death
                {
                    PlayerId = characterDeathPacket.VictimId,
                    Level = characterDeathPacket.VictimLevel,
                    ByPeekay = (byte)(playerKilledPlayer ? 1 : 0),
                    PeekayId = playerKilledPlayer ? characterDeathPacket.KillerId : 0,
                    CreatureString = characterDeathPacket.KillerName,
                    Unjust = (byte)(characterDeathPacket.Unjustified ? 0x01 : 0x00),
                    Timestamp = characterDeathPacket.Timestamp.ToFileTimeUtc()
                });

                otContext.SaveChanges();

                this.ResponsePackets.Add(new DefaultNoErrorPacket());
            }
        }
    }
}