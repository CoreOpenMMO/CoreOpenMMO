// <copyright file="FinishAuctionsResultPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using COMMO.Data.Models;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Outgoing
{
    public class FinishAuctionsResultPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)ManagementOutgoingPacketType.NoType;

        public IList<AssignedHouse> Houses { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(0x00); // Should always be 0 for this packet; means there was no error.
            message.AddUInt16((ushort)Houses.Count);

            foreach (var house in Houses)
            {
                message.AddUInt16((ushort)house.HouseId);
                message.AddUInt32((uint)house.PlayerId);
                message.AddString(house.OwnerString);
                message.AddUInt32((uint)house.Gold);
            }
        }

		public override void CleanUp() => Houses = null;
	}
}
