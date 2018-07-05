// <copyright file="CreatureTurnedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using COMMO.Communications;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Data.Contracts;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Server.Notifications
{
    internal class CreatureTurnedNotification : Notification
    {
        public ICreature Creature { get; }

        public EffectT TurnedEffect { get; }

        public CreatureTurnedNotification(Connection connection, ICreature creature, EffectT turnEffect = EffectT.None)
            : base(connection)
        {
            if (creature == null)
            {
                throw new ArgumentNullException(nameof(creature));
            }

            Creature = creature;
            TurnedEffect = turnEffect;
        }

        public override void Prepare()
        {
            if (TurnedEffect != EffectT.None)
            {
                ResponsePackets.Add(new MagicEffectPacket
                {
                    Effect = TurnedEffect,
                    Location = Creature.Location
                });
            }

            ResponsePackets.Add(new CreatureTurnedPacket
            {
                Creature = Creature
            });
        }
    }
}