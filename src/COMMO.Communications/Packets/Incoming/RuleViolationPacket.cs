// <copyright file="RuleViolationPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Incoming
{
    public class RuleViolationPacket : IPacketIncoming, IRuleViolationInfo
    {
        public RuleViolationPacket(NetworkMessage message)
        {
			GamemasterId = (int)message.GetUInt32();
			CharacterName = message.GetString();
			IpAddress = message.GetString();
			Reason = message.GetString();
			Comment = message.GetString();
        }

        public int GamemasterId { get; set; }

        public string CharacterName { get; set; }

        public string IpAddress { get; set; }

        public string Reason { get; set; }

        public string Comment { get; set; }
    }
}
