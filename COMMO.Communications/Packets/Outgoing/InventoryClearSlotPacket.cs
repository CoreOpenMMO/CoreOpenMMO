﻿// <copyright file="InventoryClearSlotPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Communications.Packets.Outgoing
{
    using COMMO.Data.Contracts;
    using COMMO.Server.Data;
    using COMMO.Server.Data.Interfaces;

    public class InventoryClearSlotPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.InventoryEmpty;

        public Slot Slot { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);

            message.AddByte((byte)this.Slot);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
