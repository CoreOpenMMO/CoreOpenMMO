// <copyright file="AuthenticationResultPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Outgoing
{
    public class AuthenticationResultPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)ManagementOutgoingPacketType.NoType;

        public bool HadError { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte((byte)(HadError ? 0x01 : 0x00));
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
