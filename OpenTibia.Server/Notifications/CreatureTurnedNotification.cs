// <copyright file="CreatureTurnedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using System;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;

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

            this.Creature = creature;
            this.TurnedEffect = turnEffect;
        }

        public override void Prepare()
        {
            if (this.TurnedEffect != EffectT.None)
            {
                this.ResponsePackets.Add(new MagicEffectPacket
                {
                    Effect = this.TurnedEffect,
                    Location = this.Creature.Location
                });
            }

            this.ResponsePackets.Add(new CreatureTurnedPacket
            {
                Creature = this.Creature
            });
        }
    }
}