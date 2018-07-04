// <copyright file="UpdateTilePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    public class UpdateTilePacket : PacketOutgoing
    {
        public Location Location { get; set; }

        public byte[] DescriptionBytes { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.TileUpdate;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);

            message.AddLocation(this.Location);

            if (this.DescriptionBytes.Length > 0)
            {
                message.AddBytes(this.DescriptionBytes);
                message.AddByte(0x00); // skip count
            }
            else
            {
                message.AddByte(0x01); // skip count
            }

            message.AddByte(0xFF);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
