// <copyright file="IItemEventLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using COMMO.Data.Contracts;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Server.Events
{
    public interface IItemEventLoader
    {
        IDictionary<ItemEventType, HashSet<IItemEvent>> Load(string moveUseFileName);
    }
}
