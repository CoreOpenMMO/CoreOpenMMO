// <copyright file="ContainerUpdateItemPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class ContainerUpdateItemPacket : PacketOutgoing
    {
        public byte Index { get; set; }

        public byte ContainerId { get; set; }

        public IItem Item { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.ContainerUpdateItem;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);

            message.AddByte(this.ContainerId);
            message.AddByte(this.Index);
            message.AddItem(this.Item);
        }

        public override void CleanUp()
        {
            this.Item = null;
        }
    }
}