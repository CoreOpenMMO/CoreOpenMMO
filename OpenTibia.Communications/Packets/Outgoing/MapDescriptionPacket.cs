// <copyright file="MapDescriptionPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    public class MapDescriptionPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.MapDescription;

        public Location Origin { get; set; }

        public byte[] DescriptionBytes { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);
            message.AddLocation(this.Origin);

            message.AddBytes(this.DescriptionBytes); // TODO: change this?
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
