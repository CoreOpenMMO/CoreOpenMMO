// <copyright file="MagicEffectPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    public class MagicEffectPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.MagicEffect;

        public Location Location { get; set; }

        public EffectT Effect { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);
            message.AddLocation(this.Location);
            message.AddByte((byte)this.Effect);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
