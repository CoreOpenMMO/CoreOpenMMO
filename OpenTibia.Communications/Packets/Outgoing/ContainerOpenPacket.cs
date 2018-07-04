// <copyright file="ContainerOpenPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class ContainerOpenPacket : PacketOutgoing
    {
        public byte ContainerId { get; set; }

        public ushort ClientItemId { get; set; }

        public string Name { get; set; }

        public byte Volume { get; set; }

        public bool HasParent { get; set; }

        public IList<IItem> Contents { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.ContainerOpen;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);

            message.AddByte(this.ContainerId);
            message.AddUInt16(this.ClientItemId);
            message.AddString(this.Name);
            message.AddByte(this.Volume);
            message.AddByte(Convert.ToByte(this.HasParent ? 0x01 : 0x00));
            message.AddByte(Convert.ToByte(this.Contents.Count));

            foreach (var item in this.Contents.Reverse())
            {
                message.AddItem(item);
            }
        }

        public override void CleanUp()
        {
            this.Contents = null;
        }
    }
}