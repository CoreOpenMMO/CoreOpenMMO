// <copyright file="PlayerLoginPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Configuration;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Incoming
{
    public class PlayerLoginPacket : IPacketIncoming, IPlayerLoginInfo
    {
        public PlayerLoginPacket(NetworkMessage message)
        {
			XteaKey = new uint[4];
			XteaKey[0] = message.GetUInt32();
			XteaKey[1] = message.GetUInt32();
			XteaKey[2] = message.GetUInt32();
			XteaKey[3] = message.GetUInt32();

			if (ServiceConfiguration.GetConfiguration().ReceivedClientVersionInt <= 770) {
				Os = message.GetUInt16();
				Version = message.GetUInt16();
			}

			IsGm = message.GetByte() > 0;

			AccountNumber = message.GetUInt32();
			CharacterName = message.GetString();
			Password = message.GetString();
        }

        public ushort Os { get; set; }

        public ushort Version { get; set; }

        public uint[] XteaKey { get; set; }

        public bool IsGm { get; set; }

        public uint AccountNumber { get; set; }

        public string CharacterName { get; set; }

        public string Password { get; set; }
    }
}
