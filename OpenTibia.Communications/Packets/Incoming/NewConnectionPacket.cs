// <copyright file="NewConnectionPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class NewConnectionPacket : IPacketIncoming, INewConnectionInfo
    {
        public NewConnectionPacket(NetworkMessage message)
        {
            this.Os = message.GetUInt16();
            this.Version = message.GetUInt16();

            message.SkipBytes(12);
        }

        public ushort Os { get; set; }

        public ushort Version { get; set; }
    }
}
