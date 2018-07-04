// <copyright file="IBuddy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Contracts
{
    public interface IBuddy
    {
        int EntryId { get; set; }

        int AccountNr { get; set; }

        int BuddyId { get; set; }

        string BuddyString { get; set; }

        long Timestamp { get; set; }

        int InitiatingId { get; set; }
    }
}
