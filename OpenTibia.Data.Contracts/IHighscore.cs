// <copyright file="IHighscore.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Contracts
{
    public interface IHighscore
    {
        int AccountId { get; set; }

        string Charname { get; set; }

        string Vocation { get; set; }

        int Level { get; set; }

        byte Exp { get; set; }

        byte Mlvl { get; set; }

        byte SkillShield { get; set; }

        byte SkillDist { get; set; }

        byte SkillAxe { get; set; }

        byte SkillSword { get; set; }

        byte SkillClub { get; set; }

        byte SkillFist { get; set; }

        byte SkillFish { get; set; }
    }
}
