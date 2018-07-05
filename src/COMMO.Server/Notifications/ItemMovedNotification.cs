// <copyright file="ItemMovedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Notifications
{
    using System;
    using COMMO.Communications;
    using COMMO.Communications.Packets.Outgoing;
    using COMMO.Server.Data.Interfaces;
    using COMMO.Server.Data.Models.Structs;

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

            Item = item;
            FromLocation = fromLocation;
            FromStackpos = fromStackPos;
            ToLocation = toLocation;
            ToStackpos = toStackPos;
            WasTeleport = wasTeleport;
        }

        public override void Prepare()
        {
            var player = Game.Instance.GetCreatureWithId(Connection.PlayerId);

            if (player.CanSee(FromLocation) && FromStackpos < 10)
            {
                ResponsePackets.Add(new RemoveAtStackposPacket
                {
                    Location = FromLocation,
                    Stackpos = FromStackpos
                });
            }

            if (player.CanSee(ToLocation))
            {
                ResponsePackets.Add(new AddItemPacket
                {
                    Location = ToLocation,
                    Item = Item
                });
            }
        }
    }
}