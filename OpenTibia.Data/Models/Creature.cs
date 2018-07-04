// <copyright file="Creature.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("creatures", Schema = "opentibia_classic")]
    public class CipCreature : ICipCreature
    {
        [Key]
        public byte Id { get; set; }

        public string Race { get; set; }

        public string Plural { get; set; }

        public string Description { get; set; }
    }
}
