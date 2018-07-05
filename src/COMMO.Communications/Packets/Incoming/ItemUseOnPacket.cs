// <copyright file="ItemUseOnPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;

namespace COMMO.Communications.Packets.Incoming
{
    public class ItemUseOnPacket : IPacketIncoming
    {
        public ItemUseOnPacket(NetworkMessage message)
        {
			FromLocation = new Location
            {
                X = message.GetUInt16(),
                Y = message.GetUInt16(),
                Z = (sbyte)message.GetByte()
            };

			FromSpriteId = message.GetUInt16();

			FromStackPosition = message.GetByte();

			ToLocation = new Location
            {
                X = message.GetUInt16(),
                Y = message.GetUInt16(),
                Z = (sbyte)message.GetByte()
            };

			ToSpriteId = message.GetUInt16();

			ToStackPosition = message.GetByte();
        }

        public Location FromLocation { get; set; }

        public ushort FromSpriteId { get; set; }

        public byte FromStackPosition { get; set; }

        public Location ToLocation { get; set; }

        public ushort ToSpriteId { get; set; }

        public byte ToStackPosition { get; set; }
    }
}
