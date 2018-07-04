// <copyright file="ServerStatusHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers.Management
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Interfaces;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Configuration;
    using OpenTibia.Data;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    internal class ServerStatusHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            this.ResponsePackets = new List<IPacketOutgoing>();

            var gameConfig = ServiceConfiguration.GetConfiguration();
            var outXmlString = string.Empty;

            var playersOnline = 0;
            var onlineRecord = 0;

            using (var otContext = new OpenTibiaDbContext())
            {
                var statusRecord = otContext.Stats.FirstOrDefault();

                if (statusRecord != null)
                {
                    playersOnline = statusRecord.PlayersOnline;
                    onlineRecord = statusRecord.RecordOnline;
                }
            }

            using (var sw = new StringWriter())
            {
                using (var writer = XmlWriter.Create(sw))
                {
                    writer.WriteStartDocument();
                    writer.WriteAttributeString("version", "1.0");
                    // writer.WriteStartElement("tsqp");
                    // writer.WriteAttributeString("version", "1.0");
                    writer.WriteStartElement("serverinfo");
                    writer.WriteAttributeString("uptime", $"{(DateTime.Now - DateTime.Today.AddHours(gameConfig.DailyResetHour)).Ticks}");
                    writer.WriteAttributeString("ip", $"{gameConfig.PublicGameIpAddress}");
                    writer.WriteAttributeString("servername", "OpenTibia"); // TODO: handle multiple game servers. gameConfig.ServerName
                    writer.WriteAttributeString("port", $"{gameConfig.PublicGamePort}");
                    writer.WriteAttributeString("location", gameConfig.LocationString); // TODO: add location to game servers...
                    writer.WriteAttributeString("url", @"http://www.randomot.com"); // TODO: make configurable gameConfig.URL
                    writer.WriteAttributeString("server", "OpenTibia"); // TODO: handle multiple game servers. gameConfig.ServerName
                    writer.WriteAttributeString("version", "0.1a"); // TODO: handle. gameConfig.GameVersion
                    writer.WriteAttributeString("client", "7.7"); // TODO: handle. gameConfig.ClientVersion
                    writer.WriteEndElement(); // "serverinfo"
                    writer.WriteStartElement("owner");
                    writer.WriteAttributeString("name", "Gamemaster"); // TODO: make configurable
                    writer.WriteAttributeString("email", "contact@randomot.com"); // TODO: make configurable
                    writer.WriteEndElement(); // "owner"
                    writer.WriteStartElement("players");
                    writer.WriteAttributeString("online", $"{playersOnline}");
                    writer.WriteAttributeString("max", $"{gameConfig.MaximumTotalPlayers}");
                    writer.WriteAttributeString("peak", $"{onlineRecord}");
                    writer.WriteEndElement(); // "players"
                    writer.WriteStartElement("motd");
                    writer.WriteString(gameConfig.MessageOfTheDay);
                    writer.WriteEndElement(); // "motd"
                    writer.WriteEndDocument();
                }

                outXmlString = sw.ToString();
            }

            this.ResponsePackets.Add(new ServerStatusPacket
            {
                Data = outXmlString
            });
        }
    }
}