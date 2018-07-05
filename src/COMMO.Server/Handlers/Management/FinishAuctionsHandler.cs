// <copyright file="FinishAuctionsHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

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

                ResponsePackets.Add(new FinishAuctionsResultPacket
                {
                    Houses = housesJustAssigned.ToList()
                });
            }
        }
    }
}