// <copyright file="IHouse.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Contracts
{
    public interface IAssignedHouse
    {
        int HouseId { get; set; }

        int PlayerId { get; set; }

        string OwnerString { get; set; }

        byte Virgin { get; set; }

        int Gold { get; set; }

        string World { get; set; }

        int PaidUntil { get; set; }

        string Grace { get; set; }

        string Guests { get; set; }

        string Subowners { get; set; }

        byte Cleanup { get; set; }

        byte Evict { get; set; }

        int PricePerSqm { get; set; }
    }
}
