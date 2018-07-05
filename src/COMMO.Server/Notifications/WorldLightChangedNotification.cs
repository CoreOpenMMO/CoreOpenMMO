// <copyright file="WorldLightChangedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Notifications
{
    using COMMO.Communications;
    using COMMO.Communications.Packets.Outgoing;

    internal class WorldLightChangedNotification : Notification
    {
        public byte LightLevel { get; }

        public byte LightColor { get; }

        public WorldLightChangedNotification(Connection connection, byte lightLevel, byte lightColor = (byte)COMMO.Data.Contracts.LightColors.White)
            : base(connection)
        {
            LightLevel = lightLevel;
            LightColor = lightColor;
        }

        public override void Prepare()
        {
            ResponsePackets.Add(new WorldLightPacket
            {
                Level = LightLevel,
                Color = LightColor
            });
        }
    }
}