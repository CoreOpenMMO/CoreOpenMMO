// <copyright file="AddCreaturePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Communications.Packets.Outgoing
{
    using COMMO.Server.Data;
    using COMMO.Server.Data.Interfaces;
    using COMMO.Server.Data.Models.Structs;

    public class AddCreaturePacket : PacketOutgoing
    {
        public Location Location { get; set; }

        public ICreature Creature { get; set; }

        public bool AsKnown { get; set; }

        public uint RemoveThisCreatureId { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.AddAtStackpos;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddLocation(Location);
            message.AddCreature(Creature, AsKnown, RemoveThisCreatureId);
        }

        public override void CleanUp()
        {
            Creature = null;
        }
    }
}
