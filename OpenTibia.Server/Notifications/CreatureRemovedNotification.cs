// <copyright file="CreatureRemovedNotification.cs" company="2Dudes">
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

    internal class CreatureRemovedNotification : Notification
    {
        public EffectT RemoveEffect { get; }

        public byte OldStackPosition { get; }

        public ICreature Creature { get; }

        public CreatureRemovedNotification(Connection connection, ICreature creature, byte oldStackPos, EffectT removeEffect = EffectT.None)
            : base(connection)
        {
            if (creature == null)
            {
                throw new ArgumentNullException(nameof(creature));
            }

            this.Creature = creature;
            this.OldStackPosition = oldStackPos;
            this.RemoveEffect = removeEffect;
        }

        public override void Prepare()
        {
            var player = Game.Instance.GetCreatureWithId(this.Connection.PlayerId);

            if (player == null || !player.CanSee(this.Creature) || !player.CanSee(this.Creature.Location))
            {
                return;
            }

            this.ResponsePackets.Add(new RemoveAtStackposPacket
            {
                Location = this.Creature.Location,
                Stackpos = this.OldStackPosition
            });

            if (this.RemoveEffect != EffectT.None)
            {
                this.ResponsePackets.Add(new MagicEffectPacket
                {
                    Location = this.Creature.Location,
                    Effect = EffectT.Puff
                });
            }
        }
    }
}