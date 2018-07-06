// <copyright file="OutfitChangedPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;

namespace COMMO.Communications.Packets.Incoming
{
    public class OutfitChangedPacket : IPacketIncoming
    {
        public OutfitChangedPacket(NetworkMessage message)
        {
            ushort lookType = message.GetUInt16();

            if (lookType != 0)
            {
				Outfit = new Outfit
                {
                    Id = lookType,
                    Head = message.GetByte(),
                    Body = message.GetByte(),
                    Legs = message.GetByte(),
                    Feet = message.GetByte()
                };
            }
            else
            {
				Outfit = new Outfit
                {
                    Id = lookType,
                    LikeType = message.GetUInt16()
                };
            }
        }

        public Outfit Outfit { get; set; }
    }
}
