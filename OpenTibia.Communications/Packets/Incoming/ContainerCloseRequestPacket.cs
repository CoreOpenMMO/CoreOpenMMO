// <copyright file="ContainerCloseRequestPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class ContainerCloseRequestPacket : IPacketIncoming
    {
        public ContainerCloseRequestPacket(NetworkMessage message)
        {
            this.ContainerId = message.GetByte();
        }

        public byte ContainerId { get; private set; }
    }
}
