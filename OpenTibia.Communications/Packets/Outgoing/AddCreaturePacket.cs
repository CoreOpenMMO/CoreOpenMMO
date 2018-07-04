// <copyright file="AddCreaturePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    public class AddCreaturePacket : PacketOutgoing
    {
        public Location Location { get; set; }

        public ICreature Creature { get; set; }

        public bool AsKnown { get; set; }

        public uint RemoveThisCreatureId { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.AddAtStackpos;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);

            message.AddLocation(this.Location);
            message.AddCreature(this.Creature, this.AsKnown, this.RemoveThisCreatureId);
        }

        public override void CleanUp()
        {
            this.Creature = null;
        }
    }
}
