// <copyright file="WorldLightPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class WorldLightPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.WorldLight;

        public byte Level { get; set; }

        public byte Color { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);

            message.AddByte(this.Level);
            message.AddByte(this.Color);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
