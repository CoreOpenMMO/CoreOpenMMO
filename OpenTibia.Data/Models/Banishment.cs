// <copyright file="Banishment.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("banishments", Schema = "opentibia_classic")]
    public class Banishment : IBanishment
    {
        [Key]
        public short BanishmentId { get; set; }

        public int AccountNr { get; set; }

        public int AccountId { get; set; }

        public string Ip { get; set; }

        public string Violation { get; set; }

        public string Comment { get; set; }

        public int Timestamp { get; set; }

        public int BanishedUntil { get; set; }

        public int GmId { get; set; }

        public byte PunishmentType { get; set; }
    }
}
