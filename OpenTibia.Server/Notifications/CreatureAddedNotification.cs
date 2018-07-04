// <copyright file="CreatureAddedNotification.cs" company="2Dudes">
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

    internal class CreatureAddedNotification : Notification
    {
        public ICreature Creature { get; }

        public EffectT AddedEffect { get; }

        public CreatureAddedNotification(Connection connection, ICreature creature, EffectT addEffect = EffectT.None)
            : base(connection)
        {
            if (creature == null)
            {
                throw new ArgumentNullException(nameof(creature));
            }

            this.Creature = creature;
            this.AddedEffect = addEffect;
        }

        public override void Prepare()
        {
            if (this.Creature.CreatureId == this.Connection.PlayerId)
            {
                return;
            }

            var player = Game.Instance.GetCreatureWithId(this.Connection.PlayerId) as IPlayer;

            if (player == null)
            {
                return;
            }

            if (this.AddedEffect != EffectT.None)
            {
                this.ResponsePackets.Add(new MagicEffectPacket
                {
                    Effect = this.AddedEffect,
                    Location = this.Creature.Location
                });
            }

            this.ResponsePackets.Add(new AddCreaturePacket
            {
                Creature = this.Creature,
                Location = this.Creature.Location,
                AsKnown = player.KnowsCreatureWithId(this.Creature.CreatureId),
                RemoveThisCreatureId = player.ChooseToRemoveFromKnownSet()
            });
        }
    }
}