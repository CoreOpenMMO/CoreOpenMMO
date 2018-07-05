// <copyright file="SectorMapLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using COMMO.Server.Data.Interfaces;
using StackExchange.Redis;

namespace COMMO.Server.Map
{
    public class SectorMapLoader : IMapLoader
    {
        // TODO: to configuration
        private static readonly Lazy<ConnectionMultiplexer> CacheConnectionInstance = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("127.0.0.1:6379"));

        public static ConnectionMultiplexer CacheConnection => CacheConnectionInstance.Value;

        public const int SectorXMin = 996;
        public const int SectorXMax = 1043;

        public const int SectorYMin = 984;
        public const int SectorYMax = 1031;

        public const int SectorZMin = 0;
        public const int SectorZMax = 15;

        private readonly DirectoryInfo mapDirInfo;
        private readonly bool[,,] sectorsLoaded;

        private long totalTileCount;
        private long totalLoadedCount;

        public byte PercentageComplete => (byte)Math.Floor(Math.Min(100, Math.Max((decimal)0, totalLoadedCount * 100 / (totalTileCount + 1))));

        public SectorMapLoader(string mapFilesPath)
        {
            if (string.IsNullOrWhiteSpace(mapFilesPath))
            {
                throw new ArgumentNullException(nameof(mapFilesPath));
            }

            mapDirInfo = new DirectoryInfo(mapFilesPath);

            totalTileCount = 1;
            totalLoadedCount = default(long);
            sectorsLoaded = new bool[SectorXMax - SectorXMin, SectorYMax - SectorYMin, SectorZMax - SectorZMin];
        }

        // public ITile[,,] LoadFullMap()
        // {
        //    return Load(SectorXMin, SectorXMax, SectorYMin, SectorYMax, SectorZMin, SectorZMax);
        // }
        public ITile[,,] Load(int fromSectorX, int toSectorX, int fromSectorY, int toSectorY, byte fromSectorZ, byte toSectorZ)
        {
            if (toSectorX < fromSectorX || toSectorY < fromSectorY || toSectorZ < fromSectorZ)
            {
                throw new InvalidOperationException("Bad range supplied.");
            }

            var tiles = new ITile[(toSectorX - fromSectorX + 1) * 32, (toSectorY - fromSectorY + 1) * 32, toSectorZ - fromSectorZ + 1];

            totalTileCount = tiles.LongLength;
            totalLoadedCount = default(long);

            IDatabase cache = CacheConnection.GetDatabase();

            Parallel.For(fromSectorZ, toSectorZ + 1, sectorZ =>
            {
                Parallel.For(fromSectorY, toSectorY + 1, sectorY =>
                {
                    Parallel.For(fromSectorX, toSectorX + 1, sectorX =>
                    {
                        var sectorFileName = $"{sectorX:0000}-{sectorY:0000}-{sectorZ:00}.sec";

                        string fileContents = cache.StringGet(sectorFileName);

                        if (fileContents == null)
                        {
                            var fullFilePath = Path.Combine(mapDirInfo.FullName, sectorFileName);
                            var sectorFileInfo = new FileInfo(fullFilePath);

                            if (sectorFileInfo.Exists)
                            {
                                using (var streamReader = sectorFileInfo.OpenText())
                                {
                                    fileContents = streamReader.ReadToEnd();

                                    cache.StringSet(sectorFileName, fileContents);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(fileContents))
                        {
                            var loadedTiles = SectorFileReader.ReadSector(sectorFileName, fileContents, (ushort)(sectorX * 32), (ushort)(sectorY * 32), (sbyte)sectorZ);

                            Parallel.ForEach(loadedTiles, tile =>
                            {
                                tiles[tile.Location.X - (fromSectorX * 32), tile.Location.Y - (fromSectorY * 32), tile.Location.Z - fromSectorZ] = tile;
                            });
                        }

                        Interlocked.Add(ref totalLoadedCount, 1024); // 1024 per sector file, regardless if there is a tile or not...
                        sectorsLoaded[sectorX - SectorXMin, sectorY - SectorYMin, sectorZ - SectorZMin] = true;
                    });
                });
            });

            totalLoadedCount = totalTileCount;

            return tiles;
        }

        public bool HasLoaded(int x, int y, byte z)
        {
            if (x > SectorXMax)
            {
                return sectorsLoaded[(x / 32) - SectorXMin, (y / 32) - SectorYMin, z - SectorZMin];
            }

            return sectorsLoaded[x - SectorXMin, y - SectorYMin, z - SectorZMin];
        }
    }
}
