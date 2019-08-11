// <copyright file="CreatureRemovedNotification.cs" company="2Dudes">
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
    internal class CreatureRemovedNotification : Notification
    {
        public EffectT RemoveEffect { get; }

        public byte OldStackPosition { get; }

        public ICreature Creature { get; }

        public CreatureRemovedNotification(Connection connection, ICreature creature, byte oldStackPos, EffectT removeEffect = EffectT.None)
            : base(connection)
        {
			Creature = creature ?? throw new ArgumentNullException(nameof(creature));
            OldStackPosition = oldStackPos;
            RemoveEffect = removeEffect;
        }

        public override void Prepare()
        {
            var player = Game.Instance.GetCreatureWithId(Connection.PlayerId);

            if (player == null || !player.CanSee(Creature) || !player.CanSee(Creature.Location))
            {
                return;
            }

            ResponsePackets.Add(new RemoveAtStackposPacket
            {
                Location = Creature.Location,
                Stackpos = OldStackPosition
            });

            if (RemoveEffect != EffectT.None)
            {
                ResponsePackets.Add(new MagicEffectPacket
                {
                    Location = Creature.Location,
                    Effect = EffectT.Puff
                });
            }
        }
    }
}