// <copyright file="Stat.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using COMMO.Data.Contracts;

namespace COMMO.Data.Models
{
    [Table("stats", Schema = "opentibia_classic")]
    public class Stat : IStat
    {
        [Key]
        public int PlayersOnline { get; set; }

        public int RecordOnline { get; set; }
    }
}
