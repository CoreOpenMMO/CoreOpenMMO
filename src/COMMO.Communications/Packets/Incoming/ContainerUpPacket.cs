// <copyright file="ContainerUpPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Incoming
{
    public class ContainerUpPacket : IPacketIncoming
    {
        public ContainerUpPacket(NetworkMessage message)
        {
			ContainerId = message.GetByte();
        }

        public byte ContainerId { get; private set; }
    }
}
