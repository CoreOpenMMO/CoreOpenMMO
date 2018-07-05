// <copyright file="ContainerRemoveItemPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Communications.Packets.Outgoing
{
    using COMMO.Server.Data;
    using COMMO.Server.Data.Interfaces;

    public class ContainerRemoveItemPacket : PacketOutgoing
    {
        public byte Index { get; set; }

        public byte ContainerId { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.ContainerRemoveItem;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddByte(ContainerId);
            message.AddByte(Index);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}