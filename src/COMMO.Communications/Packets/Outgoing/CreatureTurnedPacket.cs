// <copyright file="CreatureTurnedPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Communications.Packets.Outgoing
{
    using COMMO.Server.Data;
    using COMMO.Server.Data.Interfaces;

    public class CreatureTurnedPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.TransformThing;

        public ICreature Creature { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddLocation(Creature.Location);
            message.AddByte(Creature.GetStackPosition());
            message.AddUInt16(Creature.ThingId);
            message.AddUInt32(Creature.CreatureId);
            message.AddByte((byte)Creature.Direction);
        }

        public override void CleanUp()
        {
            Creature = null;
        }
    }
}
