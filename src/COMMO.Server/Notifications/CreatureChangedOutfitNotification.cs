// <copyright file="CreatureChangedOutfitNotification.cs" company="2Dudes">
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

    internal class CreatureChangedOutfitNotification : Notification
    {
        public ICreature Creature { get; }

        public CreatureChangedOutfitNotification(Connection connection, ICreature creature)
            : base(connection)
        {
            if (creature == null)
            {
                throw new ArgumentNullException(nameof(creature));
            }

            Creature = creature;
        }

        public override void Prepare()
        {
            ResponsePackets.Add(new CreatureChangedOutfitPacket
            {
                Creature = Creature
            });
        }
    }
}