﻿// <copyright file="IPlayerLoginInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Communications.Packets
{
    internal interface IPlayerLoginInfo
    {
        ushort Os { get; set; }

        ushort Version { get; set; }

        uint[] XteaKey { get; set; }

        bool IsGm { get; set; }

        uint AccountNumber { get; set; }

        string CharacterName { get; set; }

        string Password { get; set; }
    }
}