// <copyright file="CreatureLightPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class CreatureLightPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.CreatureLight;

        public ICreature Creature { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);

            message.AddUInt32(this.Creature.CreatureId);
            message.AddByte(this.Creature.LightBrightness); // light level
            message.AddByte(this.Creature.LightColor); // color
        }

        public override void CleanUp()
        {
            this.Creature = null;
        }
    }
}
