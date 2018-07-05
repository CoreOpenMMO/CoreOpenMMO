// <copyright file="MapPartialDescriptionPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Communications.Packets.Outgoing
{
    using System;
    using COMMO.Server.Data;
    using COMMO.Server.Data.Interfaces;

    public class MapPartialDescriptionPacket : PacketOutgoing
    {
        public override byte PacketType { get; }

        public byte[] DescriptionBytes { get; set; }

        public MapPartialDescriptionPacket(GameOutgoingPacketType mapDescriptionType)
        {
            if (mapDescriptionType != GameOutgoingPacketType.MapSliceEast &&
                mapDescriptionType != GameOutgoingPacketType.MapSliceNorth &&
                mapDescriptionType != GameOutgoingPacketType.MapSliceSouth &&
                mapDescriptionType != GameOutgoingPacketType.MapSliceWest &&
                mapDescriptionType != GameOutgoingPacketType.FloorChangeUp &&
                mapDescriptionType != GameOutgoingPacketType.FloorChangeDown)
            {
                throw new ArgumentException(nameof(mapDescriptionType));
            }

            PacketType = (byte)mapDescriptionType;
        }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddBytes(DescriptionBytes);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
