// <copyright file="WorldLightChangedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Communications;
    using OpenTibia.Communications.Packets.Outgoing;

    internal class WorldLightChangedNotification : Notification
    {
        public byte LightLevel { get; }

        public byte LightColor { get; }

        public WorldLightChangedNotification(Connection connection, byte lightLevel, byte lightColor = (byte)OpenTibia.Data.Contracts.LightColors.White)
            : base(connection)
        {
            this.LightLevel = lightLevel;
            this.LightColor = lightColor;
        }

        public override void Prepare()
        {
            this.ResponsePackets.Add(new WorldLightPacket
            {
                Level = this.LightLevel,
                Color = this.LightColor
            });
        }
    }
}