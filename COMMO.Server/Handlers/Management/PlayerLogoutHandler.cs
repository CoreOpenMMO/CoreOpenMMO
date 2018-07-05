﻿// <copyright file="PlayerLogoutHandler.cs" company="2Dudes">
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

    internal class PlayerLogoutHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var playerLogoutPacket = new ManagementPlayerLogoutPacket(message);

            using (var otContext = new OpenTibiaDbContext())
            {
                var playerRecord = otContext.Players.Where(p => p.Account_Id == playerLogoutPacket.AccountId).FirstOrDefault();

                if (playerRecord != null)
                {
                    playerRecord.Level = playerLogoutPacket.Level;
                    playerRecord.Vocation = playerLogoutPacket.Vocation;
                    playerRecord.Residence = playerLogoutPacket.Residence;
                    playerRecord.Lastlogin = playerLogoutPacket.LastLogin;

                    playerRecord.Online = 0;

                    var onlineRecord = otContext.Online.Where(o => o.Name.Equals(playerRecord.Charname)).FirstOrDefault();

                    if (onlineRecord != null)
                    {
                        otContext.Online.Remove(onlineRecord);
                    }

                    otContext.SaveChanges();

                    this.ResponsePackets.Add(new DefaultNoErrorPacket());
                }
            }
        }
    }
}