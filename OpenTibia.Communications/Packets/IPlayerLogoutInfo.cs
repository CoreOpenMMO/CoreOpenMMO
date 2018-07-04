// <copyright file="IPlayerLogoutInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets
{
    internal interface IPlayerLogoutInfo
    {
        int AccountId { get; set; }

        short Level { get; set; }

        string Vocation { get; set; }

        string Residence { get; set; }

        int LastLogin { get; set; }
    }
}