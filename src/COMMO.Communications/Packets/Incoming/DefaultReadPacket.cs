// <copyright file="DefaultReadPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Incoming
{
    public class DefaultReadPacket : IPacketIncoming
    {
        public DefaultReadPacket(NetworkMessage message)
        {
			InfoBytes = message.GetBytes(message.Length - message.Position);
        }

        public byte[] InfoBytes { get; set; }
    }
}
