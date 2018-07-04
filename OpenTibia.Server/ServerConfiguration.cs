// <copyright file="ServerConfiguration.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server
{
    public class ServerConfiguration
    {
        public const string BaseDirLocalPath = @"C:\Users\jln89\Downloads\game\";

        public const string LiveMapDirectory = BaseDirLocalPath + "map";
        public const string OriginalMapDirectory = BaseDirLocalPath + "origmap";

        public const string DataFilesDirectory = "dat";
        public const string MonsterFilesDirectory = "mon";

        public const string MoveUseFileName = "moveuse.dat";
        public const string ObjectsFileName = "objects.srv";
        public const string SpawnsFileName = "monster.db";

        public static bool SupressInvalidItemWarnings { get; set; }
    }
}
