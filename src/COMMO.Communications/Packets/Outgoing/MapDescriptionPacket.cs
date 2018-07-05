// <copyright file="MapDescriptionPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;

namespace COMMO.Communications.Packets.Outgoing
{
    public class MapDescriptionPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.MapDescription;

        public Location Origin { get; set; }

        public byte[] DescriptionBytes { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddLocation(Origin);

            message.AddBytes(DescriptionBytes); // TODO: change this?
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
