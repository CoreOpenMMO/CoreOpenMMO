// <copyright file="CreatureStat.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("creature_stat", Schema = "opentibia_classic")]
    public class CreatureStat : ICreatureStat
    {
        [Key]
        [Column(Order = 0)]
        public string Name { get; set; }

        public int KilledBy { get; set; }

        public int Killed { get; set; }

        [Key]
        [Column(Order = 1)]
        public long Time { get; set; }
    }
}
