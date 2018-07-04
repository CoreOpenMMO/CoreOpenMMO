// <copyright file="PlayerStatusPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using System;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class PlayerStatusPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.PlayerStatus;

        public IPlayer Player { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);

            message.AddUInt16((ushort)Math.Min(ushort.MaxValue, this.Player.Hitpoints));
            message.AddUInt16((ushort)Math.Min(ushort.MaxValue, this.Player.MaxHitpoints));
            message.AddUInt16(Convert.ToUInt16(this.Player.CarryStrength));

            message.AddUInt32(Math.Min(0x7FFFFFFF, this.Player.Experience)); // Experience: Client debugs after 2,147,483,647 exp

            message.AddUInt16(this.Player.Level);
            message.AddByte(this.Player.LevelPercent);
            message.AddUInt16((ushort)Math.Min(ushort.MaxValue, this.Player.Manapoints));
            message.AddUInt16((ushort)Math.Min(ushort.MaxValue, this.Player.MaxManapoints));
            message.AddByte(this.Player.GetSkillInfo(SkillType.Magic));
            message.AddByte(this.Player.GetSkillPercent(SkillType.Magic));

            message.AddByte(this.Player.SoulPoints);
        }

        public override void CleanUp()
        {
            this.Player = null;
        }
    }
}
