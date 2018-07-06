// <copyright file="TextMessagePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using COMMO.Data.Contracts;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Outgoing
{
    public class TextMessagePacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.TextMessage;

        public MessageType Type { get; set; }

        public string Message { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddByte((byte)Type);
            message.AddString(Message);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
