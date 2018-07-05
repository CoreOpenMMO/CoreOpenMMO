// <copyright file="BaseAttackOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Data.Contracts;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Notifications;

namespace COMMO.Server.Combat
{
    internal abstract class BaseAttackOperation : ICombatOperation
    {
        public ICombatActor Attacker { get; }

        public ICombatActor Target { get; }

        public abstract AttackType AttackType { get; }

        public abstract TimeSpan ExhaustionCost { get; }

        public abstract int MinimumDamage { get; }

        public abstract int MaximumDamage { get; }

        public virtual bool CanBeExecuted => (Attacker as ICreature)?.CalculateRemainingCooldownTime(CooldownType.Combat, Game.Instance.CombatSynchronizationTime) == TimeSpan.Zero;

        protected BaseAttackOperation(ICombatActor hunter, ICombatActor prey)
        {
            Attacker = hunter;
            Target = prey;
        }

        public void Execute()
        {
            EffectT resultEffect;
            bool wasShielded;
            bool wasArmorBlocked;
            TextColor textColor;

            var inflicted = InternalExecute(out resultEffect, out wasShielded, out wasArmorBlocked, out textColor);

            AnimatedTextPacket animTextPacket = null;
            var effectPacket = new MagicEffectPacket
            {
                Effect = resultEffect,
                Location = Target.Location
            };

            if (inflicted == 0)
            {
                if (wasArmorBlocked)
                {
                    effectPacket.Effect = EffectT.SparkYellow;
                }
                else if (wasShielded)
                {
                    effectPacket.Effect = EffectT.Puff;
                }
            }
            else if (inflicted > 0)
            {
                animTextPacket = new AnimatedTextPacket
                {
                    Text = inflicted.ToString(),
                    Location = Target.Location,
                    Color = textColor
                };

                if (wasShielded) // magic shield
                {
                    animTextPacket.Color = TextColor.Blue;
                    effectPacket.Effect = EffectT.RingsBlue;
                }
                else
                {
                    switch (Target.Blood)
                    {
                        case BloodType.Blood:
                            animTextPacket.Color = TextColor.Red;
                            break;
                        case BloodType.Bones:
                            animTextPacket.Color = TextColor.LightGrey;
                            break;
                        case BloodType.Fire:
                            animTextPacket.Color = TextColor.Orange;
                            break;
                        case BloodType.Slime:
                            animTextPacket.Color = TextColor.Green;
                            break;
                    }
                }
            }
            else
            {
                animTextPacket = new AnimatedTextPacket
                {
                    Text = inflicted.ToString(),
                    Location = Target.Location,
                    Color = TextColor.LightBlue
                };

                effectPacket.Effect = EffectT.GlitterBlue;
            }

            if (animTextPacket == null)
            {
                Game.Instance.NotifySpectatingPlayers(conn => new GenericNotification(conn, effectPacket), Target.Location);
            }
            else
            {
                Game.Instance.NotifySpectatingPlayers(conn => new GenericNotification(conn, effectPacket, animTextPacket), Target.Location);
            }

            Attacker.UpdateLastAttack(ExhaustionCost);
        }

        protected abstract int InternalExecute(out EffectT resultingEffect, out bool shielded, out bool armored, out TextColor colorText);
    }
}