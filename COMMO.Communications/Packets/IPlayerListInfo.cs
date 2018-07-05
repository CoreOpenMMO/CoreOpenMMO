﻿// <copyright file="IPlayerListInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Communications.Packets
{
    using System.Collections.Generic;
    using COMMO.Data.Contracts;

    internal interface IPlayerListInfo
    {
        IList<IOnlinePlayer> PlayerList { get; set; }
    }
}