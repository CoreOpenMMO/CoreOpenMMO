// <copyright file="Map.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Map
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    public class Map
    {
        private static readonly TimeSpan MapLoadPercentageReportDelay = TimeSpan.FromSeconds(7);

        // Start positions
        public static Location NewbieStart = new Location { X = 32097, Y = 32219, Z = 7 };
        public static Location VeteranStart = new Location { X = 32369, Y = 32241, Z = 7 };

        // # Refreshte Zylinder pro Minute
        // RefreshedCylinders = 8

        // # benannte Punkte
        // Mark = ("Thais",[32369,32215,7])
        // Mark = ("Carlin",[32341,31789,7])
        // Mark = ("Ab'Dendriel",[32661,31687,7])
        // Mark = ("Rookgaard",[32097,32207,7])
        // Mark = ("Fibula",[32176,32437,7])
        // Mark = ("Kazordoon",[32632,31916,8])
        // Mark = ("Senja",[32125,31667,7])
        // Mark = ("Folda",[32046,31582,7])
        // Mark = ("Vega",[32027,31692,7])
        // Mark = ("Havoc",[32783,32243,6])
        // Mark = ("Orc",[32901,31771,7])
        // Mark = ("Minocity",[32404,32124,15])
        // Mark = ("Minoroom",[32139,32109,11])
        // Mark = ("Desert",[32653,32117,7])
        // Mark = ("Swamp",[32724,31976,6])
        // Mark = ("Home",[32316,31942,7])
        // Mark = ("Mists",[32854,32333,6])
        // Mark = ("FibulaDungeon",[32189,32426,9])
        // Mark = ("DragonIsle",[32781,31603,7])
        // Mark = ("HellsGate",[32675,31648,10])
        // Mark = ("Necropolis",[32786,31683,14])
        // Mark = ("Trollcaves",[32493,32259,8])
        // Mark = ("Elvenbane",[32590,31657,7])
        // Mark = ("Fieldofglory",[32430,31671,7])
        // Mark = ("Hills",[32553,31827,6])
        // Mark = ("Sternum",[32463,32077,7])
        // Mark = ("Northport",[32486,31610,7])
        // Mark = ("Greenshore",[32273,32053,7])
        // Mark = ("Edron",[33191,31818,7])
        // Mark = ("Stonehome",[33319,31766,7])
        // Mark = ("Camp",[32655,32208,7])
        // Mark = ("Cormaya",[33302,31970,7])
        // Mark = ("Darashia",[33224,32428,7])
        // Mark = ("Drefia",[32996,32417,7])
        // Mark = ("Venore",[32955,32076,6])
        // Mark = ("Ghostship",[33325,32173,6])
        // Mark = ("VenoreDragons",[32793,32155,8])
        // Mark = ("Shadowthorn",[33086,32157,7])
        // Mark = ("Amazons",[32839,31925,7])
        // Mark = ("KingsIsle",[32174,31940,7])
        // Mark = ("Ghostlands",[32223,31831,7])
        // Mark = ("Ankrahmun",[33162,32802,7])
        // Mark = ("Oasis",[33132,32661,7])
        // Mark = ("Marid",[33103,32539,6])
        // Mark = ("Efreet",[33053,32622,6])
        // Mark = ("PortHope",[32623,32753,7])
        // Mark = ("Banuta",[32812,32559,7])
        // Mark = ("Chor",[32956,32843,7])
        // Mark = ("Trapwood",[32709,32901,8])
        // Mark = ("Eremo",[33323,31883,7])
        // Mark = ("Dagorlad",[31950,32413,7])
        // Mark = ("Poi",[32808,32337,11])

        // # Depots
        // Depot = (0,"Thais",1000)
        // Depot = (1,"Carlin",1000)
        // Depot = (2,"Kazordoon",1000)
        // Depot = (3,"Ab'Dendriel",1000)
        // Depot = (4,"Edron",1000)
        // Depot = (5,"Darashia",1000)
        // Depot = (6,"Venore",1000)
        // Depot = (7,"Ankrahmun",1000)
        // Depot = (8,"Port Hope",1000)
        private readonly ConcurrentDictionary<Location, ITile> mapTiles;

        // private Location Mininum2DLocation { get; set; }

        // private Location Maximum2DLocation { get; set; }
        public ConcurrentDictionary<Location, ITile> Tiles => this.mapTiles;

        // public bool Initialized { get; private set; }
        private IMapLoader Loader { get; }

        public Map(IMapLoader mapLoader)
        {
            if (mapLoader == null)
            {
                throw new ArgumentNullException(nameof(mapLoader));
            }

            this.Loader = mapLoader;
            // Initialized = false;
            this.mapTiles = new ConcurrentDictionary<Location, ITile>();
        }

        public void Load(Location atLocation)
        {
            var sectorX = atLocation.X / 32;
            var sectorY = atLocation.Y / 32;
            // var sectorZ = atLocation.Z;

            // Load the required sector first
            for (var z = 0; z < 15; z++)
            {
                var zByte = Convert.ToByte(z);

                if (this.Loader.HasLoaded(sectorX, sectorY, zByte))
                {
                    continue;
                }

                var loadedTiles = this.Loader.Load(sectorX, sectorX, sectorY, sectorY, zByte, zByte);

                foreach (var tile in loadedTiles)
                {
                    if (tile != null)
                    {
                        this.mapTiles[tile.Location] = tile;
                    }
                }
            }

            // asyncronously load the other sectors
            Task.Factory.StartNew(() =>
            {
                for (var x = sectorX - 1; x < sectorX + 1; x++)
                {
                    for (var y = sectorY - 1; y < sectorY + 1; y++)
                    {
                        if (x == sectorX && y == sectorY)
                        {
                            continue;
                        }

                        for (var z = 0; z < 15; z++)
                        {
                            var zByte = Convert.ToByte(z);

                            if (this.Loader.HasLoaded(x, y, zByte))
                            {
                                continue;
                            }

                            var loadedTiles = this.Loader.Load(x, x, y, y, zByte, zByte);

                            foreach (var tile in loadedTiles)
                            {
                                if (tile != null)
                                {
                                    this.mapTiles[tile.Location] = tile;
                                }
                            }
                        }
                    }
                }
            });
        }

        public void Load(int fromX = 0, int toX = 0, int fromY = 0, int toY = 0, byte fromZ = 0, byte toZ = 0)
        {
            var cts = new CancellationTokenSource();

            Task.Factory.StartNew(
                () =>
            {
                while (!cts.IsCancellationRequested)
                {
                    Console.WriteLine($"Map loading is {this.Loader.PercentageComplete}% complete.");
                    Thread.Sleep(MapLoadPercentageReportDelay);
                }
            }, cts.Token);

            // start loading.
            if (toX == 0 && toY == 0 && toZ == 0)
            {
                // Reload all map.
                // _mapTiles = Loader.LoadFullMap();
            }
            else
            {
                // else we need to validate the range.
                if (fromX < 0 || fromY < 0 || fromZ > 15 || toX < fromX || toY < fromY || toZ < fromZ)
                {
                    throw new ArgumentException("Invalid map reload range.");
                }

                var loadedTiles = this.Loader.Load(fromX, toX, fromY, toY, fromZ, toZ);

                foreach (var tile in loadedTiles)
                {
                    if (tile != null)
                    {
                        this.mapTiles[tile.Location] = tile;
                    }
                }

                // Parallel.For(0, loadedTiles.GetLength(2), z =>
                // {
                //    Parallel.For(0, loadedTiles.GetLength(1), y =>
                //    {
                //        Parallel.For(0, loadedTiles.GetLength(0), x =>
                //        {
                //            Location offsetLocation = new Location {X = x + fromX, Y = y + fromY, Z = (sbyte)(z + fromZ)};
                //            _mapTiles[offsetLocation] = loadedTiles[x, y, z];
                //        });
                //    });
                // });
            }

            cts.Cancel();
            Console.WriteLine("Map loading is complete.");

            // find out the minimum map locations in 2d...
            // for (var z = 0; z < _mapTiles.GetLength(2); z++)
            // {
            //    if (_mapTiles[0, 0, z] != null)
            //    {
            //        Mininum2DLocation = new Location
            //        {
            //            X = _mapTiles[0, 0, z].Location.X,
            //            Y = _mapTiles[0, 0, z].Location.Y,
            //            Z = 0
            //        };
            //    }

            // if (_mapTiles[_mapTiles.GetLength(0) - 1, _mapTiles.GetLength(1) - 1, z] != null)
            //    {
            //        Maximum2DLocation = new Location
            //        {
            //            X = _mapTiles[_mapTiles.GetLength(0) - 1, _mapTiles.GetLength(1) - 1, z].Location.X,
            //            Y = _mapTiles[_mapTiles.GetLength(0) - 1, _mapTiles.GetLength(1) - 1, z].Location.Y,
            //            Z = (sbyte)(_mapTiles.GetLength(2) - 1)
            //        };
            //    }
            // }

            // Initialized = true;
        }

        public ITile this[Location location]
        {
            get
            {
                // if (location < Mininum2DLocation || location > Maximum2DLocation)
                // {
                //    return null;
                // }

                // var tilesOffset = Location.GetOffsetBetween(location, Mininum2DLocation);

                // if (tilesOffset[0] >= Tiles.GetLength(0) ||
                //    tilesOffset[1] >= Tiles.GetLength(1) ||
                //    tilesOffset[2] >= Tiles.GetLength(2))
                // {
                //    return null;
                // }
                if (!this.Loader.HasLoaded(location.X, location.Y, (byte)location.Z))
                {
                    this.Load(location);
                }

                try
                {
                    return this.Tiles[location];
                }
                catch
                {
                    return null;
                }
            }
        }

        public ITile this[ushort x, ushort y, sbyte z] => this[new Location { X = x, Y = y, Z = z }];

        internal IEnumerable<uint> GetCreatureIdsAt(Location location)
        {
            var fromX = location.X - 8;
            var fromY = location.Y - 6;

            var toX = location.X + 8;
            var toY = location.Y + 6;

            var creatureList = new List<uint>();

            for (var x = fromX; x <= toX; x++)
            {
                for (var y = fromY; y <= toY; y++)
                {
                    var creaturesInTile = this[(ushort)x, (ushort)y, location.Z]?.CreatureIds;

                    if (creaturesInTile != null)
                    {
                        creatureList.AddRange(creaturesInTile);
                    }
                }
            }

            return creatureList;
        }

        public IList<byte> GetDescription(IPlayer player, ushort fromX, ushort fromY, sbyte currentZ, bool isUnderground, byte windowSizeX = MapConstants.DefaultMapWindowSizeX, byte windowSizeY = MapConstants.DefaultMapWindowSizeY)
        {
            var tempBytes = new List<byte>();

            var skip = -1;

            // we crawl from the ground up to the very top of the world (7 -> 0).
            var crawlTo = 0;
            var crawlFrom = 7;
            var crawlDelta = -1;

            // Unless... we're undeground.
            // Then we crawl from 2 floors up, this, and 2 floors down for a total of 5 floors.
            if (isUnderground)
            {
                crawlDelta = 1;
                crawlFrom = Math.Max(0, currentZ - 2);
                crawlTo = Math.Min(15, currentZ + 2);
            }

            for (var z = crawlFrom; z != crawlTo + crawlDelta; z += crawlDelta)
            {
                tempBytes.AddRange(this.GetFloorDescription(player, fromX, fromY, (sbyte)z, windowSizeX, windowSizeY, currentZ - z, ref skip));
            }

            if (skip >= 0)
            {
                tempBytes.Add((byte)skip);
                tempBytes.Add(0xFF);
            }

            return tempBytes;
        }

        public IList<byte> GetFloorDescription(IPlayer player, ushort fromX, ushort fromY, sbyte currentZ, byte windowSizeX, byte windowSizeY, int verticalOffset, ref int skip)
        {
            var tempBytes = new List<byte>();

            for (var nx = 0; nx < windowSizeX; nx++)
            {
                for (var ny = 0; ny < windowSizeY; ny++)
                {
                    var tile = this[(ushort)(fromX + nx + verticalOffset), (ushort)(fromY + ny + verticalOffset), currentZ];

                    if (tile != null)
                    {
                        if (skip >= 0)
                        {
                            tempBytes.Add((byte)skip);
                            tempBytes.Add(0xFF);
                        }

                        skip = 0;

                        tempBytes.AddRange(this.GetTileDescription(player, tile));
                    }
                    else if (++skip == 0xFF)
                    {
                        tempBytes.Add(0xFF);
                        tempBytes.Add(0xFF);
                        skip = -1;
                    }
                }
            }

            return tempBytes;
        }

        public IList<byte> GetTileDescription(IPlayer player, ITile tile)
        {
            if (tile == null)
            {
                return new byte[0];
            }

            if (tile.CachedDescription != null)
            {
                return tile.CachedDescription;
            }

            var tempBytes = new List<byte>();

            var count = 0;
            const int numberOfObjectsLimit = 9;

            if (tile.Ground != null)
            {
                tempBytes.AddRange(BitConverter.GetBytes(tile.Ground.Type.ClientId));
                count++;
            }

            foreach (var item in tile.TopItems1)
            {
                if (count == numberOfObjectsLimit)
                {
                    break;
                }

                tempBytes.AddRange(BitConverter.GetBytes(item.Type.ClientId));

                if (item.IsCumulative)
                {
                    tempBytes.Add(item.Amount);
                }
                else if (item.IsLiquidPool || item.IsLiquidContainer)
                {
                    tempBytes.Add(item.LiquidType);
                }

                count++;
            }

            foreach (var item in tile.TopItems2)
            {
                if (count == numberOfObjectsLimit)
                {
                    break;
                }

                tempBytes.AddRange(BitConverter.GetBytes(item.Type.ClientId));

                if (item.IsCumulative)
                {
                    tempBytes.Add(item.Amount);
                }
                else if (item.IsLiquidPool || item.IsLiquidContainer)
                {
                    tempBytes.Add(item.LiquidType);
                }

                count++;
            }

            foreach (var creatureId in tile.CreatureIds)
            {
                var creature = Game.Instance.GetCreatureWithId(creatureId);

                if (creature == null)
                {
                    continue;
                }

                if (count == numberOfObjectsLimit)
                {
                    break;
                }

                if (player.KnowsCreatureWithId(creatureId))
                {
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)GameOutgoingPacketType.AddKnownCreature));
                    tempBytes.AddRange(BitConverter.GetBytes(creatureId));
                }
                else
                {
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)GameOutgoingPacketType.AddUnknownCreature));
                    tempBytes.AddRange(BitConverter.GetBytes(player.ChooseToRemoveFromKnownSet()));
                    tempBytes.AddRange(BitConverter.GetBytes(creatureId));

                    player.AddKnownCreature(creatureId);

                    var creatureNameBytes = Encoding.Default.GetBytes(creature.Name);
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)creatureNameBytes.Length));
                    tempBytes.AddRange(creatureNameBytes);
                }

                tempBytes.Add((byte)Math.Min(100, creature.Hitpoints * 100 / creature.MaxHitpoints));
                tempBytes.Add((byte)creature.ClientSafeDirection);

                if (player.CanSee(creature))
                {
                    // Add creature outfit
                    tempBytes.AddRange(BitConverter.GetBytes(creature.Outfit.Id));

                    if (creature.Outfit.Id > 0)
                    {
                        tempBytes.Add(creature.Outfit.Head);
                        tempBytes.Add(creature.Outfit.Body);
                        tempBytes.Add(creature.Outfit.Legs);
                        tempBytes.Add(creature.Outfit.Feet);
                    }
                    else
                    {
                        tempBytes.AddRange(BitConverter.GetBytes(creature.Outfit.LikeType));
                    }
                }
                else
                {
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)0));
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)0));
                }

                tempBytes.Add(creature.LightBrightness);
                tempBytes.Add(creature.LightColor);

                tempBytes.AddRange(BitConverter.GetBytes(creature.Speed));

                tempBytes.Add(creature.Skull);
                tempBytes.Add(creature.Shield);
            }

            foreach (var item in tile.DownItems)
            {
                if (count == numberOfObjectsLimit)
                {
                    break;
                }

                tempBytes.AddRange(BitConverter.GetBytes(item.Type.ClientId));

                if (item.IsCumulative)
                {
                    tempBytes.Add(item.Amount);
                }
                else if (item.IsLiquidPool || item.IsLiquidContainer)
                {
                    tempBytes.Add(item.LiquidType);
                }

                count++;
            }

            return tempBytes;
        }
    }
}
