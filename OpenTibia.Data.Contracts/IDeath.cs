// <copyright file="IDeath.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Contracts
{
    public interface IDeath
    {
        int RecordId { get; set; }

        int PlayerId { get; set; }

        int Level { get; set; }

        byte ByPeekay { get; set; }

        int PeekayId { get; set; }

        string CreatureString { get; set; }

        byte Unjust { get; set; }

        long Timestamp { get; set; }
    }
}
