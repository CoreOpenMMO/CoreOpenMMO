// <copyright file="InsertHousePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class InsertHousePacket : IPacketIncoming, IInsertHouseInfo
    {
        public InsertHousePacket(NetworkMessage message)
        {
            this.Count = message.GetUInt16();
            this.HouseId = message.GetUInt16();
            this.Name = message.GetString();
            this.Rent = message.GetUInt32();
            this.Description = message.GetString();
            this.SquareMeters = message.GetUInt16();
            this.EntranceX = message.GetUInt16();
            this.EntranceY = message.GetUInt16();
            this.EntranceZ = message.GetByte();
            this.Town = message.GetString();
            this.IsGuildHouse = message.GetByte();
        }

        public ushort Count { get; set; }

        public ushort HouseId { get; set; }

        public string Name { get; set; }

        public uint Rent { get; set; }

        public string Description { get; set; }

        public ushort SquareMeters { get; set; }

        public ushort EntranceX { get; set; }

        public ushort EntranceY { get; set; }

        public byte EntranceZ { get; set; }

        public string Town { get; set; }

        public byte IsGuildHouse { get; set; }
    }
}