// <copyright file="RuleViolationPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class RuleViolationPacket : IPacketIncoming, IRuleViolationInfo
    {
        public RuleViolationPacket(NetworkMessage message)
        {
            this.GamemasterId = (int)message.GetUInt32();
            this.CharacterName = message.GetString();
            this.IpAddress = message.GetString();
            this.Reason = message.GetString();
            this.Comment = message.GetString();
        }

        public int GamemasterId { get; set; }

        public string CharacterName { get; set; }

        public string IpAddress { get; set; }

        public string Reason { get; set; }

        public string Comment { get; set; }
    }
}
