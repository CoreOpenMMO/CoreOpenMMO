﻿// <copyright file="NotationHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Handlers.Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using COMMO.Communications;
    using COMMO.Communications.Interfaces;
    using COMMO.Communications.Packets.Incoming;
    using COMMO.Communications.Packets.Outgoing;
    using COMMO.Data;
    using COMMO.Data.Models;
    using COMMO.Server.Data;
    using COMMO.Server.Data.Interfaces;

    internal class NotationHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var ruleViolationPacket = new RuleViolationPacket(message);

            using (var otContext = new OpenTibiaDbContext())
            {
                var playerRecord = otContext.Players.Where(p => p.Charname.Equals(ruleViolationPacket.CharacterName)).FirstOrDefault();

                if (playerRecord != null)
                {
                    var userRecord = otContext.Users.Where(u => u.Login == playerRecord.Account_Nr).FirstOrDefault();

                    if (userRecord != null)
                    {
                        var nowUnixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

                        otContext.Banishments.Add(new Banishment
                        {
                            AccountId = playerRecord.Account_Id,
                            AccountNr = playerRecord.Account_Nr,
                            Ip = ruleViolationPacket.IpAddress,
                            GmId = ruleViolationPacket.GamemasterId,
                            Violation = ruleViolationPacket.Reason,
                            Comment = ruleViolationPacket.Comment,
                            Timestamp = nowUnixTimestamp,
                            BanishedUntil = nowUnixTimestamp,
                            PunishmentType = 0x02
                        });

                        otContext.SaveChanges();

                        this.ResponsePackets.Add(new NotationResultPacket
                        {
                            GamemasterId = (uint)ruleViolationPacket.GamemasterId
                        });

                        return;
                    }
                }
            }

            this.ResponsePackets.Add(new DefaultErrorPacket());
        }
    }
}