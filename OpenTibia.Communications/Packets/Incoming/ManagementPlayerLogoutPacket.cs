// <copyright file="ManagementPlayerLogoutPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class ManagementPlayerLogoutPacket : IPacketIncoming, IPlayerLogoutInfo
    {
        public ManagementPlayerLogoutPacket(NetworkMessage message)
        {
            this.AccountId = (int)message.GetUInt32();
            this.Level = (short)message.GetUInt16();
            this.Vocation = message.GetString();
            this.Residence = message.GetString();
            this.LastLogin = (int)message.GetUInt32();
        }

        public int AccountId { get; set; }

        public short Level { get; set; }

        public string Vocation { get; set; }

        public string Residence { get; set; }

        public int LastLogin { get; set; }
    }
}
