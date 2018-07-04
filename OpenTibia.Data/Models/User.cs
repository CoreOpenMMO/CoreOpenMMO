// <copyright file="User.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("users", Schema = "opentibia_classic")]
    public class User : IUser
    {
        [Key]
        public short Id { get; set; }

        public int Login { get; set; }

        public string Email { get; set; }

        public string Passwd { get; set; }

        public string Session_Ip { get; set; }

        public string Last_Ip { get; set; }

        public int Creation_Ts { get; set; }

        public int Last_Ts { get; set; }

        public byte Userlevel { get; set; }

        public int Premium { get; set; }

        public int Banished { get; set; }

        public int Banished_Until { get; set; }

        public short Premium_Days { get; set; }

        public int Trial_Premium { get; set; }

        public short Trial_Premium_Days { get; set; }

        public int Bandelete { get; set; }

        public string Creation_Ip { get; set; }

        public int Lastrecover { get; set; }

        public short Posts { get; set; }

        public int Last_Post { get; set; }

        public byte Roses { get; set; }
    }
}
