// <copyright file="ManagementPlayerLoginPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class ManagementPlayerLoginPacket : IPacketIncoming, IManagementPlayerLoginInfo
    {
        public ManagementPlayerLoginPacket(NetworkMessage message)
        {
            this.AccountNumber = message.GetUInt32();
            this.CharacterName = message.GetString();
            this.Password = message.GetString();
            this.IpAddress = message.GetString();
        }

        public uint AccountNumber { get; set; }

        public string CharacterName { get; set; }

        public string Password { get; set; }

        public string IpAddress { get; set; }
    }
}
