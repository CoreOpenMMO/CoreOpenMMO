// <copyright file="CreatePlayerListHandler.cs" company="2Dudes">
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
using COMMO.Data.Models;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Server.Handlers.Management
{
    internal class CreatePlayerListHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var createPlayerListPacket = new CreatePlayerListPacket(message);

            using (var otContext = new OpenTibiaDbContext())
            {
                var currentRecord = otContext.Stats.Select(s => s.RecordOnline).FirstOrDefault();
                var isNewRecord = createPlayerListPacket.PlayerList.Count > currentRecord;

                var currentRemove = new Dictionary<string, OnlinePlayer>();

                foreach (var player in otContext.Online.ToList())
                {
                    currentRemove.Add(player.Name, player);
                }

                foreach (var player in createPlayerListPacket.PlayerList)
                {
                    var dbRecord = otContext.Online.Where(o => o.Name.Equals(player.Name)).FirstOrDefault();

                    if (dbRecord != null)
                    {
                        dbRecord.Level = player.Level;
                        dbRecord.Vocation = player.Vocation;
                    }
                    else
                    {
                        otContext.Online.Add(new OnlinePlayer
                        {
                            Name = player.Name,
                            Level = player.Level,
                            Vocation = player.Vocation
                        });
                    }

                    if (currentRemove.ContainsKey(player.Name))
                    {
                        currentRemove.Remove(player.Name);
                    }
                }

                foreach (var player in currentRemove.Values)
                {
                    otContext.Online.Remove(player);
                }

                otContext.SaveChanges();

                ResponsePackets.Add(new CreatePlayerListResultPacket
                {
                    IsNewRecord = isNewRecord
                });
            }
        }
    }
}