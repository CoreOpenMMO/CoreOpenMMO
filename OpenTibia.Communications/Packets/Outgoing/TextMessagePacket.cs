// <copyright file="TextMessagePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using System;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class TextMessagePacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.TextMessage;

        public MessageType Type { get; set; }

        public string Message { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);

            message.AddByte((byte)this.Type);
            message.AddString(this.Message);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
