// <copyright file="ContainerAddItemPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Communications.Packets.Outgoing
{
    using COMMO.Server.Data;
    using COMMO.Server.Data.Interfaces;

    public class ContainerAddItemPacket : PacketOutgoing
    {
        public byte ContainerId { get; set; }

        public IItem Item { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.ContainerAddItem;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddByte(ContainerId);
            message.AddItem(Item);
        }

        public override void CleanUp()
        {
            Item = null;
        }
    }
}