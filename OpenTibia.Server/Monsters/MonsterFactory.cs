// <copyright file="MonsterFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Monsters
{
    using System;
    using System.Collections.Generic;

    public static class MonsterFactory
    {
        public static object InitLock = new object();

        public static Dictionary<ushort, MonsterType> MonsterCatalog { get; private set; }

        public static void Initialize()
        {
            if (MonsterCatalog != null)
            {
                return;
            }

            lock (InitLock)
            {
                if (MonsterCatalog == null)
                {
                    MonsterCatalog = Game.Instance.MonsterLoader.LoadMonsters(ServerConfiguration.MonsterFilesDirectory);
                }
            }
        }

        public static Monster Create(ushort monsterId)
        {
            if (MonsterCatalog == null)
            {
                Initialize();

                if (MonsterCatalog == null)
                {
                    throw new InvalidOperationException("Failed to initialize MonsterCatalog.");
                }
            }

            if (!MonsterCatalog.ContainsKey(monsterId))
            {
                throw new ArgumentException("Invalid monster type.", nameof(monsterId));
            }

            return new Monster(MonsterCatalog[monsterId]);
        }
    }
}
