// <copyright file="Highscore.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("highscores", Schema = "opentibia_classic")]
    public class Highscore : IHighscore
    {
        [Key]
        public int AccountId { get; set; }

        public string Charname { get; set; }

        public string Vocation { get; set; }

        public int Level { get; set; }

        public byte Exp { get; set; }

        public byte Mlvl { get; set; }

        public byte SkillShield { get; set; }

        public byte SkillDist { get; set; }

        public byte SkillAxe { get; set; }

        public byte SkillSword { get; set; }

        public byte SkillClub { get; set; }

        public byte SkillFist { get; set; }

        public byte SkillFish { get; set; }
    }
}
