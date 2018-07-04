// <copyright file="IItemEventLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Events
{
    using System.Collections.Generic;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;

    public interface IItemEventLoader
    {
        IDictionary<ItemEventType, HashSet<IItemEvent>> Load(string moveUseFileName);
    }
}
