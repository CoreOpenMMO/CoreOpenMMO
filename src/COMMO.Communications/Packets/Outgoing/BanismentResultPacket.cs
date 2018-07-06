// <copyright file="BanismentResultPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Outgoing
{
    public class BanismentResultPacket : PacketOutgoing
    {
        public byte BanDays { get; set; }

        public uint BanishedUntil { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.NoType;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(0x00);
            message.AddByte(BanDays);
            message.AddUInt32(BanishedUntil);
            message.AddByte(0x00);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}