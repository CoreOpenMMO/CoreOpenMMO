// <copyright file="DefaultReadPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class DefaultReadPacket : IPacketIncoming
    {
        public DefaultReadPacket(NetworkMessage message)
        {
            this.InfoBytes = message.GetBytes(message.Length - message.Position);
        }

        public byte[] InfoBytes { get; set; }
    }
}
