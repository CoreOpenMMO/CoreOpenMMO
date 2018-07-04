// <copyright file="BanismentResultPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class BanismentResultPacket : PacketOutgoing
    {
        public byte BanDays { get; set; }

        public uint BanishedUntil { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.NoType;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(0x00);
            message.AddByte(this.BanDays);
            message.AddUInt32(this.BanishedUntil);
            message.AddByte(0x00);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}