// <copyright file="ItemUsePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;

namespace COMMO.Communications.Packets.Incoming
{
    public class ItemUsePacket : IPacketIncoming
    {
        public ItemUsePacket(NetworkMessage message)
        {
			FromLocation = new Location
            {
                X = message.GetUInt16(),
                Y = message.GetUInt16(),
                Z = (sbyte)message.GetByte()
            };

			ClientId = message.GetUInt16();
			FromStackPos = message.GetByte();

			Index = message.GetByte();
        }

        public Location FromLocation { get; private set; }

        public byte FromStackPos { get; private set; }

        public ushort ClientId { get; private set; }

        public byte Index { get; private set; }
    }
}
