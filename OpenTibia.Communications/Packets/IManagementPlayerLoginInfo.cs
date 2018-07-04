// <copyright file="IManagementPlayerLoginInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets
{
    internal interface IManagementPlayerLoginInfo
    {
        uint AccountNumber { get; set; }

        string CharacterName { get; set; }

        string Password { get; set; }

        string IpAddress { get; set; }
    }
}