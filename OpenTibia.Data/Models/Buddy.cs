// <copyright file="Buddy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("buddy", Schema = "opentibia_classic")]
    public class Buddy : IBuddy
    {
        [Key]
        public int EntryId { get; set; }

        public int AccountNr { get; set; }

        public int BuddyId { get; set; }

        public string BuddyString { get; set; }

        public long Timestamp { get; set; }

        public int InitiatingId { get; set; }
    }
}
