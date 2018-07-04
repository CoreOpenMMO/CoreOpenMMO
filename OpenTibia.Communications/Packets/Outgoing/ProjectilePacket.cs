// <copyright file="ProjectilePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    public class ProjectilePacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.CreatureMoved;

        public Location FromLocation { get; set; }

        public Location ToLocation { get; set; }

        public ShotType ShootType { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);

            message.AddLocation(this.FromLocation);
            message.AddLocation(this.ToLocation);
            message.AddByte((byte)this.ShootType);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
