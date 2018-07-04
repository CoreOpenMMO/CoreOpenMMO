// <copyright file="MonFilesMonsterLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Monsters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Items;

    public class MonFilesMonsterLoader : IMonsterLoader
    {
        /*
            A monster definition looks something like this:

            # Tibia - graphical Multi-User-Dungeon
            # MonsterRace File
            # 2005/10/12 - 16:37:51

            RaceNumber    = 68
            Name          = "vampire"
            Article       = "a"
            Outfit        = (68, 0-0-0-0)
            Corpse        = 4137
            Blood         = Bones
            Experience    = 870
            SummonCost    = 0
            FleeThreshold = 0
            Attack        = 42
            Defend        = 38
            Armor         = 27
            Poison        = 0
            LoseTarget    = 5
            Strategy      = (70, 30, 0, 0)

            Flags         = {KickBoxes,
                             KickCreatures,
                             SeeInvisible,
                             Unpushable,
                             NoSummon,
                             NoConvince,
                             NoPoison,
                             NoLifeDrain,
                             NoParalyze}

            Skills        = {(HitPoints, 450, 0, 450, 0, 0, 0),
                             (GoStrength, 70, 0, 70, 0, 0, 0),
                             (CarryStrength, 1000, 0, 1000, 0, 0, 0),
                             (FistFighting, 65, 65, 65, 100, 1500, 1)}

            Spells        = {Actor (13) -> Outfit ((122, 0-0-0-0), 6) : 120,
                             Victim (1, 0, 0) -> Damage (256, 80, 25) : 5,
                             Victim (7, 0, 14) -> Speed (-70, 20, 30) : 9}

            Inventory     = {(3027, 1, 35),
                             (2902, 1, 110),
                             (3056, 1, 7),
                             (3010, 1, 7),
                             (3031, 40, 150),
                             (3661, 1, 180),
                             (3284, 1, 9),
                             (3300, 1, 150),
                             (3559, 1, 85),
                             (3114, 1, 100),
                             (3271, 1, 15),
                             (3373, 1, 8),
                             (3434, 1, 4)}

            Talk          = {"#Y BLOOD!",
                             "Let me kiss your neck.",
                             "I smell warm blood.",
                             "I call you, my bats! Come!"}

         */

        public const char CommentSymbol = '#';
        public const char PropertyValueSeparator = '=';

        Dictionary<ushort, MonsterType> IMonsterLoader.LoadMonsters(string loadFromFile)
        {
            if (string.IsNullOrWhiteSpace(loadFromFile))
            {
                throw new ArgumentNullException(nameof(loadFromFile));
            }

            var monsFilePattern = "OpenTibia.Server.Data." + ServerConfiguration.MonsterFilesDirectory;

            var assembly = Assembly.GetExecutingAssembly();

            var monsterDictionary = new Dictionary<ushort, MonsterType>();
            var monsterFilePaths = assembly.GetManifestResourceNames().Where(s => s.Contains(monsFilePattern));

            foreach (var monsterFilePath in monsterFilePaths)
            {
                using (var stream = assembly.GetManifestResourceStream(monsterFilePath))
                {
                    if (stream == null)
                    {
                        throw new Exception($"Failed to load {monsterFilePath}.");
                    }

                    using (var reader = new StreamReader(stream))
                    {
                        var dataList = new List<Tuple<string, string>>();

                        var propName = string.Empty;
                        var propData = string.Empty;

                        foreach (var readLine in reader.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                        {
                            var inLine = readLine?.Split(new[] { ObjectsFileItemLoader.CommentSymbol }, 2).FirstOrDefault();

                            // ignore comments and empty lines.
                            if (string.IsNullOrWhiteSpace(inLine))
                            {
                                continue;
                            }

                            var data = inLine.Split(new[] { ObjectsFileItemLoader.PropertyValueSeparator }, 2);

                            if (data.Length > 2)
                            {
                                throw new Exception($"Malformed line [{inLine}] in objects file: [{monsterFilePath}]");
                            }

                            if (data.Length == 1)
                            {
                                // line is a continuation of the last prop.
                                propData += data[0].ToLower().Trim();
                            }
                            else
                            {
                                if (propName.Length > 0 && propData.Length > 0)
                                {
                                    dataList.Add(new Tuple<string, string>(propName, propData));
                                }

                                propName = data[0].ToLower().Trim();
                                propData = data[1].Trim();
                            }
                        }

                        if (propName.Length > 0 && propData.Length > 0)
                        {
                            dataList.Add(new Tuple<string, string>(propName, propData));
                        }

                        var current = new MonsterType();

                        foreach (var tuple in dataList)
                        {
                            switch (tuple.Item1)
                            {
                                case "racenumber":
                                    current.SetId(Convert.ToUInt16(tuple.Item2));
                                    break;
                                case "name":
                                    current.SetName(tuple.Item2.Trim('\"'));
                                    break;
                                case "article":
                                    current.SetArticle(tuple.Item2.Trim('\"'));
                                    break;
                                case "outfit":
                                    current.SetOutfit(tuple.Item2.Trim('(', ')'));
                                    break;
                                case "corpse":
                                    current.SetCorpse(Convert.ToUInt16(tuple.Item2));
                                    break;
                                case "blood":
                                    current.SetBlood(tuple.Item2);
                                    break;
                                case "experience":
                                    current.SetExperience(Convert.ToUInt32(tuple.Item2));
                                    break;
                                case "summoncost":
                                    current.SetSummonCost(Convert.ToUInt16(tuple.Item2));
                                    break;
                                case "fleethreshold":
                                    current.SetFleeTreshold(Convert.ToUInt16(tuple.Item2));
                                    break;
                                case "attack":
                                    current.SetAttack(Convert.ToUInt16(tuple.Item2));
                                    break;
                                case "defend":
                                    current.SetDefend(Convert.ToUInt16(tuple.Item2));
                                    break;
                                case "armor":
                                    current.SetArmor(Convert.ToUInt16(tuple.Item2));
                                    break;
                                case "poison":
                                    current.SetConditionInfect(Convert.ToUInt16(tuple.Item2), ConditionType.Posion);
                                    break;
                                case "losetarget":
                                    current.SetLoseTarget(Convert.ToByte(tuple.Item2));
                                    break;
                                case "strategy":
                                    current.SetStrategy(tuple.Item2.Trim('(', ')'));
                                    break;
                                case "flags":
                                    current.SetFlags(tuple.Item2.Trim('{', '}'));
                                    break;
                                case "skills":
                                    current.SetSkills(tuple.Item2.Trim('{', '}'));
                                    break;
                                case "spells":
                                    current.SetSpells(tuple.Item2.Trim('{', '}'));
                                    break;
                                case "inventory":
                                    current.SetInventory(tuple.Item2.Trim('{', '}'));
                                    break;
                                case "talk":
                                    current.SetPhrases(tuple.Item2.Trim('{', '}'));
                                    break;
                            }
                        }

                        current.LockChanges();
                        monsterDictionary.Add(current.RaceId, current);
                    }
                }
            }

            return monsterDictionary;
        }

        /// <summary>
        /// Assumes map has been loaded.
        /// </summary>
        /// <param name="spawnsFileName"></param>
        /// <returns></returns>
        public IEnumerable<Spawn> LoadSpawns(string spawnsFileName)
        {
            if (string.IsNullOrWhiteSpace(spawnsFileName))
            {
                throw new ArgumentNullException(nameof(spawnsFileName));
            }

            var spawns = new List<Spawn>();
            var spawnsFilePath = "OpenTibia.Server.Data." + ServerConfiguration.DataFilesDirectory + "." + spawnsFileName;

            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(spawnsFilePath))
            {
                if (stream == null)
                {
                    throw new Exception($"Failed to load {spawnsFilePath}.");
                }

                using (var reader = new StreamReader(stream))
                {
                    foreach (var readLine in reader.ReadToEnd().Split("\r\n".ToCharArray()))
                    {
                        var inLine = readLine?.Split(new[] { ObjectsFileItemLoader.CommentSymbol }, 2).FirstOrDefault();

                        // ignore comments and empty lines.
                        if (string.IsNullOrWhiteSpace(inLine))
                        {
                            continue;
                        }

                        var data = inLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (data.Length != 7)
                        {
                            // ignore malformed lines.
                            continue;
                        }

                        var id = Convert.ToUInt16(data[0]);
                        var loc = new Location
                        {
                            X = Convert.ToInt32(data[1]),
                            Y = Convert.ToInt32(data[2]),
                            Z = Convert.ToSByte(data[3])
                        };
                        var radius = Convert.ToUInt16(data[4]);
                        var monCount = Convert.ToByte(data[5]);
                        var regen = TimeSpan.FromSeconds(Convert.ToUInt32(data[6]));

                        spawns.Add(new Spawn
                        {
                            Id = id,
                            Location = loc,
                            Radius = radius,
                            Count = monCount,
                            Regen = regen
                        });
                    }
                }
            }

            return spawns;
        }
    }
}
