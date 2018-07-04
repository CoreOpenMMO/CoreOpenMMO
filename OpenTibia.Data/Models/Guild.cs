// <copyright file="Guild.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("guilds", Schema = "opentibia_classic")]
    public class Guild : IGuild
    {
        [Key]
        public short GuildId { get; set; }

        public string GuildName { get; set; }

        public int GuildOwner { get; set; }

        public string Description { get; set; }

        public int Ts { get; set; }

        public byte Ranks { get; set; }

        public string Rank1 { get; set; }

        public string Rank2 { get; set; }

        public string Rank3 { get; set; }

        public string Rank4 { get; set; }

        public string Rank5 { get; set; }

        public string Rank6 { get; set; }

        public string Rank7 { get; set; }

        public string Rank8 { get; set; }

        public string Rank9 { get; set; }

        public string Rank10 { get; set; }

        public string Logo { get; set; }
    }
}
