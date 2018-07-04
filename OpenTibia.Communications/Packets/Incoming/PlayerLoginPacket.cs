// <copyright file="PlayerLoginPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class PlayerLoginPacket : IPacketIncoming, IPlayerLoginInfo
    {
        public PlayerLoginPacket(NetworkMessage message)
        {
            this.XteaKey = new uint[4];
            this.XteaKey[0] = message.GetUInt32();
            this.XteaKey[1] = message.GetUInt32();
            this.XteaKey[2] = message.GetUInt32();
            this.XteaKey[3] = message.GetUInt32();

            this.Os = message.GetUInt16();
            this.Version = message.GetUInt16();

            this.IsGm = message.GetByte() > 0;

            this.AccountNumber = message.GetUInt32();
            this.CharacterName = message.GetString();
            this.Password = message.GetString();
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
