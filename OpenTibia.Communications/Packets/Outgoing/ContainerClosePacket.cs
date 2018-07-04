// <copyright file="ContainerClosePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class ContainerClosePacket : PacketOutgoing
    {
        public byte ContainerId { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.ContainerClose;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);

            message.AddByte(this.ContainerId);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}