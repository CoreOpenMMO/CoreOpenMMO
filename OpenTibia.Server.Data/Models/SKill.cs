// <copyright file="SKill.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Data.Models
{
    using System;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;

    public class Skill : ISkill
    {
        public event OnLevelAdvance OnAdvance;

        public SkillType Type { get; }

        public ushort Level { get; private set; }

        public ushort MaxLevel { get; }

        public ushort DefaultLevel { get; }

        public double Count { get; private set; }

        public double Rate { get; }

        public double Target { get; private set; }

        public double BaseIncrease { get; }

        public Skill(SkillType type, ushort defaultLevel, double rate, double baseIncrease, ushort level = 0, ushort maxLevel = 1, double count = 0)
        {
            if (defaultLevel < 1)
            {
                throw new InvalidModelException($"{nameof(defaultLevel)} must be positive.");
            }

            if (maxLevel < 1)
            {
                throw new InvalidModelException($"{nameof(maxLevel)} must be positive.");
            }

            if (rate < 1)
            {
                throw new InvalidModelException($"{nameof(rate)} must be positive.");
            }

            if (baseIncrease < 1)
            {
                throw new InvalidModelException($"{nameof(baseIncrease)} must be positive.");
            }

            if (count < 0)
            {
                throw new InvalidModelException($"{nameof(count)} cannot be negative.");
            }

            if (maxLevel < defaultLevel)
            {
                throw new InvalidModelException($"{nameof(maxLevel)} must be at least the same value as {nameof(defaultLevel)}.");
            }

            this.Type = type;
            this.DefaultLevel = defaultLevel;
            this.MaxLevel = maxLevel;
            this.Level = Math.Min(this.MaxLevel, level == 0 ? defaultLevel : level);
            this.Rate = rate;
            this.BaseIncrease = baseIncrease;

            this.Target = this.CalculateNextTarget();
            this.Count = Math.Min(count, this.Target);
        }

        private double CalculateNextTarget()
        {
            var nextTarget = (this.Target * this.Rate) + this.BaseIncrease;

            if (Math.Abs(this.Target) < 0.001) // need to recalculate everything.
            {
                for (int i = 0; i < this.DefaultLevel - this.Level; i++) // how many advances we need to calculate
                {
                    nextTarget = (nextTarget * this.Rate) + this.BaseIncrease;
                }
            }

            return nextTarget;
        }

        public void IncreaseCounter(double value)
        {
            this.Count = Math.Min(this.Target, this.Count + value);

            if (Math.Abs(this.Count - this.Target) < 0.001) // Skill level advance
            {
                this.Level++;
                this.Target = this.CalculateNextTarget();

                // Invoke any subscribers to the level advance.
                this.OnAdvance?.Invoke(this.Type);
            }
        }
    }
}