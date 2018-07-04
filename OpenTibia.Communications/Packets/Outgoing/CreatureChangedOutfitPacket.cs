// <copyright file="CreatureChangedOutfitPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class CreatureChangedOutfitPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.CreatureOutfit;

        public ICreature Creature { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);
            message.AddUInt32(this.Creature.CreatureId);

            message.AddUInt16(this.Creature.Outfit.Id);

            if (this.Creature.Outfit.Id != 0)
            {
                message.AddByte(this.Creature.Outfit.Head);
                message.AddByte(this.Creature.Outfit.Body);
                message.AddByte(this.Creature.Outfit.Legs);
                message.AddByte(this.Creature.Outfit.Feet);
            }
            else
            {
                message.AddUInt16(this.Creature.Outfit.LikeType);
            }
        }

        public override void CleanUp()
        {
            this.Creature = null;
        }
    }
}
