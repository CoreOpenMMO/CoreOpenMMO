// <copyright file="AssignedHouse.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("houses", Schema = "opentibia_classic")]
    public class AssignedHouse : IAssignedHouse
    {
        [Key]
        public int HouseId { get; set; }

        public int PlayerId { get; set; }

        public string OwnerString { get; set; }

        public byte Virgin { get; set; }

        public int Gold { get; set; }

        public string World { get; set; }

        public int PaidUntil { get; set; }

        public string Grace { get; set; }

        public string Guests { get; set; }

        public string Subowners { get; set; }

        public byte Cleanup { get; set; }

        public byte Evict { get; set; }

        public int PricePerSqm { get; set; }
    }
}
