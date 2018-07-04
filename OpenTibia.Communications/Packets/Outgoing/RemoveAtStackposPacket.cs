// <copyright file="RemoveAtStackposPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    public class RemoveAtStackposPacket : PacketOutgoing
    {
        public Location Location { get; set; }

        public byte Stackpos { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.RemoveAtStackpos;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);
            message.AddLocation(this.Location);
            message.AddByte(this.Stackpos);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
