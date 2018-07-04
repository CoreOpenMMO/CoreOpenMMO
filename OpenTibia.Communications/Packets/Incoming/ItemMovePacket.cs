// <copyright file="ItemMovePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    public class ItemMovePacket : IPacketIncoming
    {
        public ItemMovePacket(NetworkMessage message)
        {
            this.FromLocation = new Location
            {
                X = message.GetUInt16(),
                Y = message.GetUInt16(),
                Z = (sbyte)message.GetByte()
            };

            this.ClientId = message.GetUInt16();
            this.FromStackPos = message.GetByte();

            this.ToLocation = new Location
            {
                X = message.GetUInt16(),
                Y = message.GetUInt16(),
                Z = (sbyte)message.GetByte()
            };

            this.Count = message.GetByte();
        }

        public Location FromLocation { get; private set; }

        public Location ToLocation { get; private set; }

        public byte FromStackPos { get; private set; }

        public ushort ClientId { get; private set; }

        public byte Count { get; private set; }
    }
}
