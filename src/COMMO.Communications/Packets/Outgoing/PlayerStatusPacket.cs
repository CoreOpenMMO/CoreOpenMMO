// <copyright file="PlayerStatusPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using COMMO.Data.Contracts;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Outgoing
{
    public class PlayerStatusPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.PlayerStatus;

        public IPlayer Player { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddUInt16((ushort)Math.Min(ushort.MaxValue, Player.Hitpoints));
            message.AddUInt16((ushort)Math.Min(ushort.MaxValue, Player.MaxHitpoints));
            message.AddUInt16(Convert.ToUInt16(Player.CarryStrength));

            message.AddUInt32(Math.Min(0x7FFFFFFF, Player.Experience)); // Experience: Client debugs after 2,147,483,647 exp

            message.AddUInt16(Player.Level);
            message.AddByte(Player.LevelPercent);
            message.AddUInt16((ushort)Math.Min(ushort.MaxValue, Player.Manapoints));
            message.AddUInt16((ushort)Math.Min(ushort.MaxValue, Player.MaxManapoints));
            message.AddByte(Player.GetSkillInfo(SkillType.Magic));
            message.AddByte(Player.GetSkillPercent(SkillType.Magic));

            message.AddByte(Player.SoulPoints);
        }

		public override void CleanUp() => Player = null;
	}
}
