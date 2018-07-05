// <copyright file="ManagementPlayerLogoutPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Incoming
{
    public class ManagementPlayerLogoutPacket : IPacketIncoming, IPlayerLogoutInfo
    {
        public ManagementPlayerLogoutPacket(NetworkMessage message)
        {
			AccountId = (int)message.GetUInt32();
			Level = (short)message.GetUInt16();
			Vocation = message.GetString();
			Residence = message.GetString();
			LastLogin = (int)message.GetUInt32();
        }

        public int AccountId { get; set; }

        public short Level { get; set; }

        public string Vocation { get; set; }

        public string Residence { get; set; }

        public int LastLogin { get; set; }
    }
}
