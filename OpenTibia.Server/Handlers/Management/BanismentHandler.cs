// <copyright file="BanismentHandler.cs" company="2Dudes">
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
    using OpenTibia.Data.Models;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    internal class BanismentHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var ruleViolationPacket = new RuleViolationPacket(message);

            byte banDays = 0;
            var banUntilDate = DateTime.Now;

            using (var otContext = new OpenTibiaDbContext())
            {
                var playerRecord = otContext.Players.Where(p => p.Charname.Equals(ruleViolationPacket.CharacterName)).FirstOrDefault();

                if (playerRecord != null)
                {
                    var userRecord = otContext.Users.Where(u => u.Login == playerRecord.Account_Nr).FirstOrDefault();

                    if (userRecord != null)
                    {
                        // Calculate Banishment date based on number of previous banishments youger than 60 days...
                        var todayMinus60Days = DateTime.Today.AddDays(-60).ToFileTimeUtc();
                        var banCount = otContext.Banishments.Where(b => b.AccountNr == playerRecord.Account_Nr && b.Timestamp > todayMinus60Days && b.PunishmentType == 1).Count();

                        switch (banCount)
                        {
                            case 0:
                                banDays = 7;
                                break;
                            case 1:
                                banDays = 15;
                                break;
                            case 2:
                                banDays = 30;
                                break;
                            case 3:
                                banDays = 90;
                                break;
                            default:
                                banDays = 255;
                                break;
                        }

                        banUntilDate = banUntilDate.AddDays(banDays);
                        var nowUnixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                        var banUntilUnixTimestamp = (int)banUntilDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

                        otContext.Banishments.Add(new Banishment
                        {
                            AccountId = playerRecord.Account_Id,
                            AccountNr = playerRecord.Account_Nr,
                            Ip = ruleViolationPacket.IpAddress,
                            GmId = ruleViolationPacket.GamemasterId,
                            Violation = ruleViolationPacket.Reason,
                            Comment = ruleViolationPacket.Comment,
                            Timestamp = nowUnixTimestamp,
                            BanishedUntil = banUntilUnixTimestamp,
                            PunishmentType = 1
                        });

                        userRecord.Banished = 1;
                        userRecord.Banished_Until = banUntilUnixTimestamp;

                        otContext.SaveChanges();

                        this.ResponsePackets.Add(new BanismentResultPacket
                        {
                            BanDays = banDays,
                            BanishedUntil = (uint)banUntilDate.ToFileTimeUtc()
                        });

                        return;
                    }
                }
            }

            this.ResponsePackets.Add(new DefaultErrorPacket());
        }
    }
}