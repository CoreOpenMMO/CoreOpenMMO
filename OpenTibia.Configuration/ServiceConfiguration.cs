// <copyright file="ServiceConfiguration.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Configuration
{
    using System.Net;
    using OpenTibia.Data.Contracts;

    public class ServiceConfiguration
    {
        public WorldType WorldType { get; set; }

        public IPAddress PublicGameIpAddress { get; set; }

        public ushort PublicGamePort { get; set; }

        public byte DailyResetHour { get; set; }

        public ushort MaximumTotalPlayers { get; set; }

        public ushort PremiumMainlandBuffer { get; set; }

        public ushort MaximumRookgardians { get; set; }

        public ushort PremiumRookgardiansBuffer { get; set; }

        public string QueryManagerPassword { get; set; }

        public string MessageOfTheDay { get; set; }

        public string LocationString { get; set; }

        public int GameVersionInt { get; set; }

        public string GameVersionString { get; set; }

        public int ClientVersionInt { get; set; }

        public string ClientVersionString { get; set; }

        public string WebsiteUrl { get; set; }

        public IPAddress PrivateGameIpAddress { get; set; }

        public ushort PrivateGamePort { get; set; }

        public bool UsingCipsoftRsaKeys { get; set; }

        public string WorldName { get; set; }

        private static readonly object ConfigLock = new object();
        private static ServiceConfiguration config;

        public static ServiceConfiguration GetConfiguration()
        {
            if (config == null)
            {
                lock (ConfigLock)
                {
                    if (config == null)
                    {
                        config = new ServiceConfiguration
                        {
                            UsingCipsoftRsaKeys = true,
                            GameVersionInt = 10,
                            GameVersionString = "0.1",
                            ClientVersionInt = 770,
                            ClientVersionString = "7.7",
                            WorldType = WorldType.Hardcore,
                            PublicGameIpAddress = IPAddress.Parse("127.0.0.1"),
                            PublicGamePort = 7172,
                            PrivateGameIpAddress = IPAddress.Parse("10.0.0.5"),
                            PrivateGamePort = 7170,
                            DailyResetHour = 10,
                            MaximumTotalPlayers = 1500,
                            PremiumMainlandBuffer = 900,
                            MaximumRookgardians = 500,
                            PremiumRookgardiansBuffer = 250,
                            QueryManagerPassword = "a6glaf0c",
                            LocationString = "United States",
                            MessageOfTheDay = "Welcome to OpenTibia.",
                            WebsiteUrl = @"http://www.randomot.com",
                            WorldName = "Open Tibia"
                        };
                    }
                }
            }

            return config;
        }
    }
}
