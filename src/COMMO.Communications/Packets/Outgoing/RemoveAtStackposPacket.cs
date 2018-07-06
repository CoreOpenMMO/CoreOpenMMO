// <copyright file="RemoveAtStackposPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;

namespace COMMO.Communications.Packets.Outgoing
{
    public class RemoveAtStackposPacket : PacketOutgoing
    {
        public Location Location { get; set; }

        public byte Stackpos { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.RemoveAtStackpos;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddLocation(Location);
            message.AddByte(Stackpos);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
