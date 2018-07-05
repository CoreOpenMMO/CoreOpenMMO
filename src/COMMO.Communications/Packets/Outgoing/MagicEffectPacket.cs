// <copyright file="MagicEffectPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Communications.Packets.Outgoing
{
    using COMMO.Data.Contracts;
    using COMMO.Server.Data;
    using COMMO.Server.Data.Interfaces;
    using COMMO.Server.Data.Models.Structs;

    public class MagicEffectPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.MagicEffect;

        public Location Location { get; set; }

        public EffectT Effect { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddLocation(Location);
            message.AddByte((byte)Effect);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
