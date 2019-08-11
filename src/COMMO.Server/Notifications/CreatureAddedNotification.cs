// <copyright file="CreatureAddedNotification.cs" company="2Dudes">
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
    internal class CreatureAddedNotification : Notification
    {
        public ICreature Creature { get; }

        public EffectT AddedEffect { get; }

        public CreatureAddedNotification(Connection connection, ICreature creature, EffectT addEffect = EffectT.None)
            : base(connection)
        {
			Creature = creature ?? throw new ArgumentNullException(nameof(creature));
            AddedEffect = addEffect;
        }

        public override void Prepare()
        {
            if (Creature.CreatureId == Connection.PlayerId)
            {
                return;
            }


			if (!(Game.Instance.GetCreatureWithId(Connection.PlayerId) is IPlayer player)) {
				return;
			}

			if (AddedEffect != EffectT.None)
            {
                ResponsePackets.Add(new MagicEffectPacket
                {
                    Effect = AddedEffect,
                    Location = Creature.Location
                });
            }

            ResponsePackets.Add(new AddCreaturePacket
            {
                Creature = Creature,
                Location = Creature.Location,
                AsKnown = player.KnowsCreatureWithId(Creature.CreatureId),
                RemoveThisCreatureId = player.ChooseToRemoveFromKnownSet()
            });
        }
    }
}