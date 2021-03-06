// <copyright file="AnimatedTextPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Data.Contracts;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;

namespace COMMO.Communications.Packets.Outgoing
{
    public class AnimatedTextPacket : PacketOutgoing
    {
        public Location Location { get; set; }

        public TextColor Color { get; set; }

        public string Text { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.AnimatedText;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddLocation(Location);
            message.AddByte((byte)Color);
            message.AddString(Text);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
