// <copyright file="ContainerOpenPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Outgoing
{
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
            message.AddByte(PacketType);

            message.AddByte(ContainerId);
            message.AddUInt16(ClientItemId);
            message.AddString(Name);
            message.AddByte(Volume);
            message.AddByte(Convert.ToByte(HasParent ? 0x01 : 0x00));
            message.AddByte(Convert.ToByte(Contents.Count));

            foreach (var item in Contents.Reverse())
            {
                message.AddItem(item);
            }
        }

        public override void CleanUp()
        {
            Contents = null;
        }
    }
}