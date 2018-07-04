// <copyright file="ItemMovedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using System;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    internal class ItemMovedNotification : Notification
    {
        public bool WasTeleport { get; }

        public byte FromStackpos { get; }

        public byte ToStackpos { get; }

        public Location FromLocation { get; }

        public Location ToLocation { get; }

        public IItem Item { get; }

        public ItemMovedNotification(Connection connection, IItem item, Location fromLocation, byte fromStackPos, Location toLocation, byte toStackPos, bool wasTeleport)
            : base(connection)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.Item = item;
            this.FromLocation = fromLocation;
            this.FromStackpos = fromStackPos;
            this.ToLocation = toLocation;
            this.ToStackpos = toStackPos;
            this.WasTeleport = wasTeleport;
        }

        public override void Prepare()
        {
            var player = Game.Instance.GetCreatureWithId(this.Connection.PlayerId);

            if (player.CanSee(this.FromLocation) && this.FromStackpos < 10)
            {
                this.ResponsePackets.Add(new RemoveAtStackposPacket
                {
                    Location = this.FromLocation,
                    Stackpos = this.FromStackpos
                });
            }

            if (player.CanSee(this.ToLocation))
            {
                this.ResponsePackets.Add(new AddItemPacket
                {
                    Location = this.ToLocation,
                    Item = this.Item
                });
            }
        }
    }
}