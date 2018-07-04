// <copyright file="IMonsterLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Monsters
{
    using System.Collections.Generic;
    using OpenTibia.Server.Data.Models.Structs;

    public interface IMonsterLoader
    {
        Dictionary<ushort, MonsterType> LoadMonsters(string loadFromPath);

        IEnumerable<Spawn> LoadSpawns(string spawnsFileName);
    }
}
