// <copyright file="PacketOutgoing.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public abstract class PacketOutgoing : IPacketOutgoing
    {
        public abstract byte PacketType { get; }

        public abstract void Add(NetworkMessage message);

        public abstract void CleanUp();
    }
}
