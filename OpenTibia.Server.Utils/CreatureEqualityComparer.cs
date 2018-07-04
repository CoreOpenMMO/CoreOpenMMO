// <copyright file="CreatureEqualityComparer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Utilities
{
    using System.Collections.Generic;
    using OpenTibia.Server.Data.Interfaces;

    public class CreatureEqualityComparer : IEqualityComparer<ICreature>
    {
        public bool Equals(ICreature x, ICreature y)
        {
            return x.CreatureId == y.CreatureId;
        }

        public int GetHashCode(ICreature obj)
        {
            return (int)obj.CreatureId;
        }
    }
}
