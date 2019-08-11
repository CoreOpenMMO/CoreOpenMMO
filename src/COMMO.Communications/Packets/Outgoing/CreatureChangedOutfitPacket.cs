// <copyright file="CreatureChangedOutfitPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Outgoing
{
    public class CreatureChangedOutfitPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.CreatureOutfit;

        public ICreature Creature { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddUInt32(Creature.CreatureId);

            message.AddUInt16(Creature.Outfit.Id);

            if (Creature.Outfit.Id != 0)
            {
                message.AddByte(Creature.Outfit.Head);
                message.AddByte(Creature.Outfit.Body);
                message.AddByte(Creature.Outfit.Legs);
                message.AddByte(Creature.Outfit.Feet);
            }
            else
            {
                message.AddUInt16(Creature.Outfit.LikeType);
            }
        }

		public override void CleanUp() => Creature = null;
	}
}
