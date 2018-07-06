// <copyright file="ManagementPlayerLoginPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Incoming
{
    public class ManagementPlayerLoginPacket : IPacketIncoming, IManagementPlayerLoginInfo
    {
        public ManagementPlayerLoginPacket(NetworkMessage message)
        {
			AccountNumber = message.GetUInt32();
			CharacterName = message.GetString();
			Password = message.GetString();
			IpAddress = message.GetString();
        }

        public uint AccountNumber { get; set; }

        public string CharacterName { get; set; }

        public string Password { get; set; }

        public string IpAddress { get; set; }
    }
}
