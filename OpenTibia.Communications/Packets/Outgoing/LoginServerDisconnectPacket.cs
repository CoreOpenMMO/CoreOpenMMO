// <copyright file="LoginServerDisconnectPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class LoginServerDisconnectPacket : PacketOutgoing
    {
        public string Reason { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.Disconnect;

        public LoginServerDisconnectPacket()
        {
            this.Reason = string.Empty;
        }

        public LoginServerDisconnectPacket(string reason)
        {
            this.Reason = reason;
        }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);
            message.AddString(this.Reason);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
