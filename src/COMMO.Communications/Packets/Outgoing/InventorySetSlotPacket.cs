// <copyright file="InventorySetSlotPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Data.Contracts;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Outgoing
{
    public class InventorySetSlotPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.InventoryItem;

        public Slot Slot { get; set; }

        public IItem Item { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddByte((byte)Slot);
            message.AddItem(Item);
        }

        public override void CleanUp()
        {
            Item = null;
        }
    }
}
