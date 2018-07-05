// <copyright file="ServerStatusPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;
using COMMO.Utilities;

namespace COMMO.Communications.Packets.Outgoing
{
    public class ServerStatusPacket : PacketOutgoing
    {
        public string Data { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.NoType;

        public override void Add(NetworkMessage message)
        {
            message.AddBytes(Data.ToByteArray());
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
