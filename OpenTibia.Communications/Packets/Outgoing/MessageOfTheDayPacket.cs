// <copyright file="MessageOfTheDayPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class MessageOfTheDayPacket : PacketOutgoing
    {
        public string MessageOfTheDay { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.MessageOfTheDay;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);
            message.AddString("1\n" + this.MessageOfTheDay);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
