// <copyright file="IUser.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Contracts
{
    public interface IUser
    {
        short Id { get; set; }

        int Login { get; set; }

        string Email { get; set; }

        string Passwd { get; set; }

        string Session_Ip { get; set; }

        string Last_Ip { get; set; }

        int Creation_Ts { get; set; }

        int Last_Ts { get; set; }

        byte Userlevel { get; set; }

        int Premium { get; set; }

        int Banished { get; set; }

        int Banished_Until { get; set; }

        short Premium_Days { get; set; }

        int Trial_Premium { get; set; }

        short Trial_Premium_Days { get; set; }

        int Bandelete { get; set; }

        string Creation_Ip { get; set; }

        int Lastrecover { get; set; }

        short Posts { get; set; }

        int Last_Post { get; set; }

        byte Roses { get; set; }
    }
}
