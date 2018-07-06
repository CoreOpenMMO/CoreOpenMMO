// <copyright file="TileUpdatedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Communications;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Server.Data.Models.Structs;

namespace COMMO.Server.Notifications
{
    internal class TileUpdatedNotification : Notification
    {
        public Location Location { get; }

        public byte[] Description { get; }

        public TileUpdatedNotification(Connection connection, Location location, byte[] description)
            : base(connection)
        {
            Location = location;
            Description = description;
        }

        public override void Prepare()
        {
            ResponsePackets.Add(new UpdateTilePacket
            {
                Location = Location,
                DescriptionBytes = Description
            });
        }
    }
}