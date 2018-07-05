// <copyright file="ProjectilePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Communications.Packets.Outgoing
{
    using COMMO.Data.Contracts;
    using COMMO.Server.Data;
    using COMMO.Server.Data.Interfaces;
    using COMMO.Server.Data.Models.Structs;

    public class ProjectilePacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.CreatureMoved;

        public Location FromLocation { get; set; }

        public Location ToLocation { get; set; }

        public ShotType ShootType { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddLocation(FromLocation);
            message.AddLocation(ToLocation);
            message.AddByte((byte)ShootType);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
