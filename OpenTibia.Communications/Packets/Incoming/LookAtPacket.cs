// <copyright file="LookAtPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    public class LookAtPacket : IPacketIncoming
    {
        public LookAtPacket(NetworkMessage message)
        {
            this.Location = new Location
            {
                X = message.GetUInt16(),
                Y = message.GetUInt16(),
                Z = (sbyte)message.GetByte()
            };

            this.ThingId = message.GetUInt16();
            this.StackPosition = message.GetByte();
        }

        public Location Location { get; private set; }

        public ushort ThingId { get; private set; }

        public byte StackPosition { get; private set; }
    }
}
