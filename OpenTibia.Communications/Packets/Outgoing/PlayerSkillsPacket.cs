// <copyright file="PlayerSkillsPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class PlayerSkillsPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.PlayerSkills;

        public IPlayer Player { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);

            message.AddByte(this.Player.GetSkillInfo(SkillType.Fist));
            message.AddByte(this.Player.GetSkillPercent(SkillType.Fist));

            message.AddByte(this.Player.GetSkillInfo(SkillType.Club));
            message.AddByte(this.Player.GetSkillPercent(SkillType.Club));

            message.AddByte(this.Player.GetSkillInfo(SkillType.Sword));
            message.AddByte(this.Player.GetSkillPercent(SkillType.Sword));

            message.AddByte(this.Player.GetSkillInfo(SkillType.Axe));
            message.AddByte(this.Player.GetSkillPercent(SkillType.Axe));

            message.AddByte(this.Player.GetSkillInfo(SkillType.Ranged));
            message.AddByte(this.Player.GetSkillPercent(SkillType.Ranged));

            message.AddByte(this.Player.GetSkillInfo(SkillType.Shield));
            message.AddByte(this.Player.GetSkillPercent(SkillType.Shield));

            message.AddByte(this.Player.GetSkillInfo(SkillType.Fishing));
            message.AddByte(this.Player.GetSkillPercent(SkillType.Fishing));
        }

        public override void CleanUp()
        {
            this.Player = null;
        }
    }
}
