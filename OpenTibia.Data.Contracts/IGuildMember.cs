// <copyright file="IGuildMember.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Contracts
{
    public interface IGuildMember
    {
        int EntryId { get; set; }

        int AccountId { get; set; }

        short GuildId { get; set; }

        string GuildTitle { get; set; }

        string PlayerTitle { get; set; }

        byte Invitation { get; set; }

        int Timestamp { get; set; }

        byte Rank { get; set; }
    }
}
