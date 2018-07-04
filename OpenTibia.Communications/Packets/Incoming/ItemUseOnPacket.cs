// <copyright file="ItemUseOnPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    public class ItemUseOnPacket : IPacketIncoming
    {
        public ItemUseOnPacket(NetworkMessage message)
        {
            this.FromLocation = new Location
            {
                X = message.GetUInt16(),
                Y = message.GetUInt16(),
                Z = (sbyte)message.GetByte()
            };

            this.FromSpriteId = message.GetUInt16();

            this.FromStackPosition = message.GetByte();

            this.ToLocation = new Location
            {
                X = message.GetUInt16(),
                Y = message.GetUInt16(),
                Z = (sbyte)message.GetByte()
            };

            this.ToSpriteId = message.GetUInt16();

            this.ToStackPosition = message.GetByte();
        }

        public Location FromLocation { get; set; }

        public ushort FromSpriteId { get; set; }

        public byte FromStackPosition { get; set; }

        public Location ToLocation { get; set; }

        public ushort ToSpriteId { get; set; }

        public byte ToStackPosition { get; set; }
    }
}
