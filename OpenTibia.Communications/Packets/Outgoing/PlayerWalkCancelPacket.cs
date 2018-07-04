// <copyright file="PlayerWalkCancelPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class PlayerWalkCancelPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.PlayerWalkCancel;

        public Direction Direction { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);
            message.AddByte((byte)this.Direction);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
