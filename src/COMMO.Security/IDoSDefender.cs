﻿// <copyright file="IDoSDefender.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Security
{
    internal interface IDoSDefender
    {
        void AddToBlocked(string addessStr);

        bool IsBlockedAddress(string addressStr);
    }
}