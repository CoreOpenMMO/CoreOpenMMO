// <copyright file="MonsterType.cs" company="2Dudes">
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
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Parsing;

    public enum MonsterSkill : byte
    {
        HITPOINTS,
        GOSTRENGTH,
        CARRYSTRENGTH,
        FISTFIGHTING,
        SOULPOINTS,
        EATING,
        FISHING,
        AXEFIGHTING,
        CLUBFIGHTING,
        SWORDFIGHTING,
        DISTANCEFIGHTING,
        SHIELDING,
        MANA,
        MAGICLEVEL,
        LEVEL
    }

    public class MonsterType
    {
        public bool Locked { get; private set; }

        public ushort RaceId { get; private set; }

        public string Name { get; private set; }

        public string Article { get; private set; }

        public uint Experience { get; private set; }

        public ushort SummonCost { get; private set; }

        public ushort FleeThreshold { get; private set; }

        public byte LoseTarget { get; private set; }

        public ushort ConditionInfect { get; } // Holds ConditionType that this monster infects upon dealt damage.

        public HashSet<KnownSpell> KnownSpells { get; }

        public HashSet<CreatureFlag> Flags { get; }

        public Dictionary<SkillType, ISkill> Skills { get; }

        public List<string> Phrases { get; }

        public List<Tuple<ushort, byte, ushort>> InventoryComposition { get; }

        public Tuple<byte, byte, byte, byte> Strategy { get; private set; }

        public ushort Attack { get; private set; }

        public ushort Defense { get; private set; }

        public ushort Armor { get; private set; }

        public Outfit Outfit { get; private set; }

        public ushort Corpse { get; private set; }

        public uint MaxHitPoints { get; private set; }

        public uint MaxManaPoints { get; }

        public BloodType Blood { get; private set; }

        public ushort Speed { get; private set; }

        public ushort Capacity { get; private set; }

        public MonsterType()
        {
            this.RaceId = 0;
            this.Name = string.Empty;
            this.MaxManaPoints = 0;

            this.Attack = 1;
            this.Defense = 1;
            this.Armor = 1;

            this.Experience = 0;
            this.SummonCost = 0;
            this.FleeThreshold = 0;
            this.LoseTarget = 0;
            this.ConditionInfect = 0;

            this.Flags = new HashSet<CreatureFlag>();
            this.KnownSpells = new HashSet<KnownSpell>();
            this.Phrases = new List<string>();
            this.Skills = new Dictionary<SkillType, ISkill>();
            this.InventoryComposition = new List<Tuple<ushort, byte, ushort>>();

            this.Locked = false;
        }

        public void LockChanges()
        {
            this.Locked = true;
        }

        public void SetId(ushort id)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            this.RaceId = id;
        }

        public void SetName(string name)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            this.Name = name;
        }

        internal void SetArticle(string article)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            this.Article = article;
        }

        internal void SetOutfit(string outfitStr)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            // comes in the form
            // 68, 0-0-0-0
            var splitStr = outfitStr.Split(new[] { ',' }, 2);
            var outfitId = Convert.ToUInt16(splitStr[0]);

            var outfitSections = splitStr[1].Split('-').Select(s => Convert.ToByte(s)).ToArray();

            if (outfitId == 0)
            {
                this.Outfit = new Outfit
                {
                    Id = outfitId,
                    LikeType = outfitSections[0]
                };
            }
            else
            {
                this.Outfit = new Outfit
                {
                    Id = outfitId,
                    Head = outfitSections[0],
                    Body = outfitSections[1],
                    Legs = outfitSections[2],
                    Feet = outfitSections[3]
                };
            }
        }

        internal void SetCorpse(ushort corpse)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            this.Corpse = corpse;
        }

        internal void SetBlood(string propData)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            BloodType bloodType;

            if (Enum.TryParse(propData, out bloodType))
            {
                this.Blood = bloodType;
            }
        }

        internal void SetExperience(uint experience)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            this.Experience = experience;
        }

        internal void SetSummonCost(ushort summonCost)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            this.SummonCost = summonCost;
        }

        internal void SetFleeTreshold(ushort fleeThreshold)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            this.FleeThreshold = fleeThreshold;
        }

        internal void SetDefend(ushort defense)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            this.Defense = defense;
        }

        internal void SetArmor(ushort armor)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            this.Armor = armor;
        }

        internal void SetAttack(ushort attack)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            this.Attack = attack;
        }

        internal void SetConditionInfect(ushort conditionValue, ConditionType posion)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            // TODO: implement.
        }

        internal void SetLoseTarget(byte loseTarget)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            this.LoseTarget = loseTarget;
        }

        internal void SetStrategy(string strategyStr)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            // comes in the form
            // 70, 30, 0, 0
            var strat = strategyStr.Split(',').Select(s => Convert.ToByte(s)).ToArray();

            if (strat.Length != 4)
            {
                throw new InvalidDataException($"Unexpected number of elements in Strategy value {strategyStr} on monster type {this.Name}.");
            }

            this.Strategy = new Tuple<byte, byte, byte, byte>(strat[0], strat[1], strat[2], strat[3]);
        }

        internal void SetFlags(string flagsStr)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            foreach (var flagName in flagsStr.Split(','))
            {
                if (string.IsNullOrWhiteSpace(flagName))
                {
                    continue;
                }

                CreatureFlag creatureFlag;

                if (Enum.TryParse(flagName, out creatureFlag))
                {
                    this.Flags.Add(creatureFlag);
                }
            }
        }

        internal void SetSpells(string v)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            // TODO: implement.
        }

        internal void SetSkills(string v)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            var enclosingChars = new Dictionary<char, char>
            {
                { CipParser.CloseCurly, CipParser.OpenCurly },
                { CipParser.CloseParenthesis, CipParser.OpenParenthesis }
            };

            var enclosures = CipParser.GetEnclosedButPreserveQuoted(v, enclosingChars);

            foreach (var enclosure in enclosures)
            {
                var skillParams = enclosure.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (!skillParams.Any())
                {
                    continue;
                }

                if (skillParams.Length != 7)
                {
                    throw new InvalidDataException($"Unexpected number of elements in skill line {v} on monster type {this.Name}.");
                }

                MonsterSkill mSkill;

                if (!Enum.TryParse(skillParams[0].ToUpper(), out mSkill))
                {
                    continue;
                }

                switch (mSkill)
                {
                    case MonsterSkill.HITPOINTS:
                        this.MaxHitPoints = Convert.ToUInt32(skillParams[1]);
                        break;
                    case MonsterSkill.GOSTRENGTH:
                        try
                        {
                            this.Speed = Convert.ToUInt16(skillParams[1]);
                        }
                        catch
                        {
                            // TODO: handle -1 here...
                        }

                        break;
                    case MonsterSkill.CARRYSTRENGTH:
                        this.Capacity = Convert.ToUInt16(skillParams[1]);
                        break;
                    case MonsterSkill.FISTFIGHTING:
                        var fistLevel = Convert.ToUInt16(skillParams[1]);
                        if (fistLevel > 0)
                        {
                            this.Skills[SkillType.Fist] = new Skill(SkillType.Fist, fistLevel, 1.00, 1, fistLevel, (ushort)(fistLevel * 2));
                        }

                        break;
                    case MonsterSkill.AXEFIGHTING:
                        var axeLevel = Convert.ToUInt16(skillParams[1]);
                        if (axeLevel > 0)
                        {
                            this.Skills[SkillType.Fist] = new Skill(SkillType.Fist, axeLevel, 1.00, 1, axeLevel, (ushort)(axeLevel * 2));
                        }

                        break;
                    case MonsterSkill.SWORDFIGHTING:
                        var swordLevel = Convert.ToUInt16(skillParams[1]);
                        if (swordLevel > 0)
                        {
                            this.Skills[SkillType.Fist] = new Skill(SkillType.Fist, swordLevel, 1.00, 1, swordLevel, (ushort)(swordLevel * 2));
                        }

                        break;
                    case MonsterSkill.CLUBFIGHTING:
                        var clubLevel = Convert.ToUInt16(skillParams[1]);
                        if (clubLevel > 0)
                        {
                            this.Skills[SkillType.Fist] = new Skill(SkillType.Fist, clubLevel, 1.00, 1, clubLevel, (ushort)(clubLevel * 2));
                        }

                        break;
                }
            }
        }

        internal void SetInventory(string v)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            var enclosingChars = new Dictionary<char, char>
            {
                { CipParser.CloseCurly, CipParser.OpenCurly },
                { CipParser.CloseParenthesis, CipParser.OpenParenthesis }
            };

            var enclosures = CipParser.GetEnclosedButPreserveQuoted(v, enclosingChars);

            foreach (var enclosure in enclosures)
            {
                var inventoryParams = enclosure.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (!inventoryParams.Any())
                {
                    continue;
                }

                if (inventoryParams.Length != 3)
                {
                    throw new InvalidDataException($"Unexpected number of elements in inventory line {v} on monster type {this.Name}.");
                }

                this.InventoryComposition.Add(new Tuple<ushort, byte, ushort>(Convert.ToUInt16(inventoryParams[0]), Convert.ToByte(inventoryParams[1]), Convert.ToUInt16(inventoryParams[2])));
            }
        }

        internal void SetPhrases(string v)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            this.Phrases.AddRange(CipParser.SplitByTokenPreserveQuoted(v, ','));
        }
    }
}