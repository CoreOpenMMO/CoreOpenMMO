// <copyright file="SelfAppearPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Outgoing
{
    public class SelfAppearPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.SelfAppear;

        public uint CreatureId { get; set; }

        public byte GraphicsSpeed => 0x32; // Should always be 32 apparently...

        public byte CanReportBugs => 0x00;

        public bool IsLogin { get; set; }

        public IPlayer Player { get; set; }

        // public HashSet<string> Privileges { get; set; }
        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddUInt32(CreatureId);
            message.AddByte(GraphicsSpeed);
            message.AddByte(CanReportBugs);

            message.AddByte(Math.Min((byte)1, Player.AccessLevel));

            if (Player.AccessLevel > 0) // TODO: WTF are these, permissions?
            {
                message.AddByte(0x0B);

                for (var i = 0; i < 32; i++)
                {
                    message.AddByte(0xFF);
                }
            }
        }

		public override void CleanUp() => Player = null;
	}
}
