// <copyright file="BaseAttackOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Combat
{
    using System;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Notifications;

    internal abstract class BaseAttackOperation : ICombatOperation
    {
        public ICombatActor Attacker { get; }

        public ICombatActor Target { get; }

        public abstract AttackType AttackType { get; }

        public abstract TimeSpan ExhaustionCost { get; }

        public abstract int MinimumDamage { get; }

        public abstract int MaximumDamage { get; }

        public virtual bool CanBeExecuted => (this.Attacker as ICreature)?.CalculateRemainingCooldownTime(CooldownType.Combat, Game.Instance.CombatSynchronizationTime) == TimeSpan.Zero;

        protected BaseAttackOperation(ICombatActor hunter, ICombatActor prey)
        {
            this.Attacker = hunter;
            this.Target = prey;
        }

        public void Execute()
        {
            EffectT resultEffect;
            bool wasShielded;
            bool wasArmorBlocked;
            TextColor textColor;

            var inflicted = this.InternalExecute(out resultEffect, out wasShielded, out wasArmorBlocked, out textColor);

            AnimatedTextPacket animTextPacket = null;
            var effectPacket = new MagicEffectPacket
            {
                Effect = resultEffect,
                Location = this.Target.Location
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
                    Location = this.Target.Location,
                    Color = textColor
                };

                if (wasShielded) // magic shield
                {
                    animTextPacket.Color = TextColor.Blue;
                    effectPacket.Effect = EffectT.RingsBlue;
                }
                else
                {
                    switch (this.Target.Blood)
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
                    Location = this.Target.Location,
                    Color = TextColor.LightBlue
                };

                effectPacket.Effect = EffectT.GlitterBlue;
            }

            if (animTextPacket == null)
            {
                Game.Instance.NotifySpectatingPlayers(conn => new GenericNotification(conn, effectPacket), this.Target.Location);
            }
            else
            {
                Game.Instance.NotifySpectatingPlayers(conn => new GenericNotification(conn, effectPacket, animTextPacket), this.Target.Location);
            }

            this.Attacker.UpdateLastAttack(this.ExhaustionCost);
        }

        protected abstract int InternalExecute(out EffectT resultingEffect, out bool shielded, out bool armored, out TextColor colorText);
    }
}