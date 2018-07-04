// <copyright file="StandardAttackOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Combat
{
    using System;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;

    internal class StandardAttackOperation : BaseAttackOperation
    {
        public StandardAttackOperation(ICombatActor hunter, ICombatActor prey)
            : base(hunter, prey)
        {
        }

        public override bool CanBeExecuted
        {
            get
            {
                if (this.Target == null || !base.CanBeExecuted)
                {
                    return false;
                }

                var locationDiff = this.Attacker.Location - this.Target.Location;

                return locationDiff.Z == 0 && this.Attacker.AutoAttackRange >= locationDiff.MaxValueIn2D;
            }
        }

        public override AttackType AttackType => AttackType.Physical; // TODO: wands, firesword, poison stuff, etc?

        public override TimeSpan ExhaustionCost => TimeSpan.FromSeconds(2);

        public override int MinimumDamage
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int MaximumDamage
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override int InternalExecute(out EffectT resultingEffect, out bool shielded, out bool armored, out TextColor colorText)
        {
            resultingEffect = EffectT.XBlood;
            colorText = TextColor.Red;
            shielded = false;
            armored = false;

            var rng = new Random((int)this.Attacker.ActorId);

            var val = rng.Next(4);

            switch (val)
            {
                default:
                    return 0;
                case 1:
                    break;
                case 2:
                    shielded = true;
                    break;
                case 3:
                    armored = true;
                    break;
            }

            return rng.Next(10) + 1;
        }
    }
}