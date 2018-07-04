// <copyright file="OutfitChangedPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    public class OutfitChangedPacket : IPacketIncoming
    {
        public OutfitChangedPacket(NetworkMessage message)
        {
            ushort lookType = message.GetUInt16();

            if (lookType != 0)
            {
                this.Outfit = new Outfit
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
                this.Outfit = new Outfit
                {
                    Id = lookType,
                    LikeType = message.GetUInt16()
                };
            }
        }

        public Outfit Outfit { get; set; }
    }
}
