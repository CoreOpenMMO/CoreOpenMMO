// <copyright file="ISkill.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Data.Interfaces
{
    using OpenTibia.Data.Contracts;

    public delegate void OnLevelAdvance(SkillType skillType);

    public interface ISkill
    {
        event OnLevelAdvance OnAdvance;

        SkillType Type { get; }

        ushort Level { get; }

        ushort MaxLevel { get; }

        ushort DefaultLevel { get; }

        double Count { get; }

        double Rate { get; }

        double Target { get; }

        double BaseIncrease { get; }

        void IncreaseCounter(double value);
    }
}