// <copyright file="LoginServerDisconnectPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Outgoing
{
    public class LoginServerDisconnectPacket : PacketOutgoing
    {
        public string Reason { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.Disconnect;

        public LoginServerDisconnectPacket()
        {
            Reason = string.Empty;
        }

        public LoginServerDisconnectPacket(string reason)
        {
            Reason = reason;
        }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddString(Reason);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
