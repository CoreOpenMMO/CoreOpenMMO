// <copyright file="PlayerSkillsPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Data.Contracts;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Outgoing
{
    public class PlayerSkillsPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.PlayerSkills;

        public IPlayer Player { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddByte(Player.GetSkillInfo(SkillType.Fist));
            message.AddByte(Player.GetSkillPercent(SkillType.Fist));

            message.AddByte(Player.GetSkillInfo(SkillType.Club));
            message.AddByte(Player.GetSkillPercent(SkillType.Club));

            message.AddByte(Player.GetSkillInfo(SkillType.Sword));
            message.AddByte(Player.GetSkillPercent(SkillType.Sword));

            message.AddByte(Player.GetSkillInfo(SkillType.Axe));
            message.AddByte(Player.GetSkillPercent(SkillType.Axe));

            message.AddByte(Player.GetSkillInfo(SkillType.Ranged));
            message.AddByte(Player.GetSkillPercent(SkillType.Ranged));

            message.AddByte(Player.GetSkillInfo(SkillType.Shield));
            message.AddByte(Player.GetSkillPercent(SkillType.Shield));

            message.AddByte(Player.GetSkillInfo(SkillType.Fishing));
            message.AddByte(Player.GetSkillPercent(SkillType.Fishing));
        }

        public override void CleanUp()
        {
            Player = null;
        }
    }
}
