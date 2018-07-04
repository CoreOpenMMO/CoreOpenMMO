// <copyright file="GuildMember.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("guild_members", Schema = "opentibia_classic")]
    public class GuildMember : IGuildMember
    {
        [Key]
        public int EntryId { get; set; }

        public int AccountId { get; set; }

        public short GuildId { get; set; }

        public string GuildTitle { get; set; }

        public string PlayerTitle { get; set; }

        public byte Invitation { get; set; }

        public int Timestamp { get; set; }

        public byte Rank { get; set; }
    }
}
