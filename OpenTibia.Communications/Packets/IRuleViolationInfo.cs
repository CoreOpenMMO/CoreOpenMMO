// <copyright file="IRuleViolationInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets
{
    internal interface IRuleViolationInfo
    {
        int GamemasterId { get; set; }

        string CharacterName { get; set; }

        string IpAddress { get; set; }

        string Reason { get; set; }

        string Comment { get; set; }
    }
}