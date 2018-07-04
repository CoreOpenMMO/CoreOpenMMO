// <copyright file="IBanishment.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Contracts
{
    public interface IBanishment
    {
        short BanishmentId { get; set; }

        int AccountNr { get; set; }

        int AccountId { get; set; }

        string Ip { get; set; }

        string Violation { get; set; }

        string Comment { get; set; }

        int Timestamp { get; set; }

        int BanishedUntil { get; set; }

        int GmId { get; set; }

        byte PunishmentType { get; set; }
    }
}
