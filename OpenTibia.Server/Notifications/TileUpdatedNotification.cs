// <copyright file="TileUpdatedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Communications;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Data.Models.Structs;

    internal class TileUpdatedNotification : Notification
    {
        public Location Location { get; }

        public byte[] Description { get; }

        public TileUpdatedNotification(Connection connection, Location location, byte[] description)
            : base(connection)
        {
            this.Location = location;
            this.Description = description;
        }

        public override void Prepare()
        {
            this.ResponsePackets.Add(new UpdateTilePacket
            {
                Location = this.Location,
                DescriptionBytes = this.Description
            });
        }
    }
}