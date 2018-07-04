// <copyright file="OnlinePlayer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("online", Schema = "opentibia_classic")]
    public class OnlinePlayer : IOnlinePlayer
    {
        [Key]
        public string Name { get; set; }

        public int Level { get; set; }

        public string Vocation { get; set; }
    }
}
