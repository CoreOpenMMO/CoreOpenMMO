// <copyright file="CreatureTurnedPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class CreatureTurnedPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.TransformThing;

        public ICreature Creature { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);

            message.AddLocation(this.Creature.Location);
            message.AddByte(this.Creature.GetStackPosition());
            message.AddUInt16(this.Creature.ThingId);
            message.AddUInt32(this.Creature.CreatureId);
            message.AddByte((byte)this.Creature.Direction);
        }

        public override void CleanUp()
        {
            this.Creature = null;
        }
    }
}
