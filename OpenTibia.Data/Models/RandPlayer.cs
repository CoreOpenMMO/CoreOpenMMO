// <copyright file="RandPlayer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("rand_players", Schema = "opentibia_classic")]
    public class RandPlayer : IRandPlayer
    {
        [Key]
        public int RandId { get; set; }

        public int AccountId { get; set; }

        public int Order { get; set; }

        public int AssignedTo { get; set; }
    }
}
