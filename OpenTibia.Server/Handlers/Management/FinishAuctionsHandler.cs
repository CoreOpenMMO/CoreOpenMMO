// <copyright file="FinishAuctionsHandler.cs" company="2Dudes">
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

    internal class FinishAuctionsHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var finishAuctionsPacket = new DefaultReadPacket(message);

            using (var otContext = new OpenTibiaDbContext())
            {
                var housesJustAssigned = otContext.AssignedHouses.Where(h => h.Virgin > 0);

                foreach (var house in housesJustAssigned)
                {
                    house.Virgin = 0;
                }

                otContext.SaveChanges();

                this.ResponsePackets.Add(new FinishAuctionsResultPacket
                {
                    Houses = housesJustAssigned.ToList()
                });
            }
        }
    }
}