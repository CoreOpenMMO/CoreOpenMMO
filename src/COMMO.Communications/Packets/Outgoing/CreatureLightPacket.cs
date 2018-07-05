// <copyright file="CreatureLightPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Communications.Packets.Outgoing
{
    using COMMO.Server.Data;
    using COMMO.Server.Data.Interfaces;

    public class CreatureLightPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.CreatureLight;

        public ICreature Creature { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddUInt32(Creature.CreatureId);
            message.AddByte(Creature.LightBrightness); // light level
            message.AddByte(Creature.LightColor); // color
        }

        public override void CleanUp()
        {
            Creature = null;
        }
    }
}
