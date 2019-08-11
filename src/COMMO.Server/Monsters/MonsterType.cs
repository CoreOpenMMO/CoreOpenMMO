// <copyright file="MonsterType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using COMMO.Data.Contracts;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models;
using COMMO.Server.Data.Models.Structs;
using COMMO.Server.Parsing;

namespace COMMO.Server.Monsters
{
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
            RaceId = 0;
            Name = string.Empty;
            MaxManaPoints = 0;

            Attack = 1;
            Defense = 1;
            Armor = 1;

            Experience = 0;
            SummonCost = 0;
            FleeThreshold = 0;
            LoseTarget = 0;
            ConditionInfect = 0;

            Flags = new HashSet<CreatureFlag>();
            KnownSpells = new HashSet<KnownSpell>();
            Phrases = new List<string>();
            Skills = new Dictionary<SkillType, ISkill>();
            InventoryComposition = new List<Tuple<ushort, byte, ushort>>();

            Locked = false;
        }

		public void LockChanges() => Locked = true;

		public void SetId(ushort id)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            RaceId = id;
        }

        public void SetName(string name)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            Name = name;
        }

        internal void SetArticle(string article)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            Article = article;
        }

        internal void SetOutfit(string outfitStr)
        {
            if (Locked)
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
                Outfit = new Outfit
                {
                    Id = outfitId,
                    LikeType = outfitSections[0]
                };
            }
            else
            {
                Outfit = new Outfit
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
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            Corpse = corpse;
        }

        internal void SetBlood(string propData)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }


			if (Enum.TryParse(propData, out
			BloodType bloodType)) {
				Blood = bloodType;
			}
		}

        internal void SetExperience(uint experience)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            Experience = experience;
        }

        internal void SetSummonCost(ushort summonCost)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            SummonCost = summonCost;
        }

        internal void SetFleeTreshold(ushort fleeThreshold)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            FleeThreshold = fleeThreshold;
        }

        internal void SetDefend(ushort defense)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            Defense = defense;
        }

        internal void SetArmor(ushort armor)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            Armor = armor;
        }

        internal void SetAttack(ushort attack)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            Attack = attack;
        }

        internal void SetConditionInfect(ushort conditionValue, ConditionType posion)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            // TODO: implement.
        }

        internal void SetLoseTarget(byte loseTarget)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            LoseTarget = loseTarget;
        }

        internal void SetStrategy(string strategyStr)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            // comes in the form
            // 70, 30, 0, 0
            var strat = strategyStr.Split(',').Select(s => Convert.ToByte(s)).ToArray();

            if (strat.Length != 4)
            {
                throw new InvalidDataException($"Unexpected number of elements in Strategy value {strategyStr} on monster type {Name}.");
            }

            Strategy = new Tuple<byte, byte, byte, byte>(strat[0], strat[1], strat[2], strat[3]);
        }

        internal void SetFlags(string flagsStr)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            foreach (var flagName in flagsStr.Split(','))
            {
                if (string.IsNullOrWhiteSpace(flagName))
                {
                    continue;
                }


				if (Enum.TryParse(flagName, out
				CreatureFlag creatureFlag)) {
					Flags.Add(creatureFlag);
				}
			}
        }

        internal void SetSpells(string v)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            // TODO: implement.
        }

        internal void SetSkills(string v)
        {
            if (Locked)
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
                    throw new InvalidDataException($"Unexpected number of elements in skill line {v} on monster type {Name}.");
                }


				if (!Enum.TryParse(skillParams[0].ToUpper(), out
				MonsterSkill mSkill)) {
					continue;
				}

				switch (mSkill)
                {
                    case MonsterSkill.HITPOINTS:
                        MaxHitPoints = Convert.ToUInt32(skillParams[1]);
                        break;
                    case MonsterSkill.GOSTRENGTH:
                        try
                        {
                            Speed = Convert.ToUInt16(skillParams[1]);
                        }
                        catch
                        {
                            // TODO: handle -1 here...
                        }

                        break;
                    case MonsterSkill.CARRYSTRENGTH:
                        Capacity = Convert.ToUInt16(skillParams[1]);
                        break;
                    case MonsterSkill.FISTFIGHTING:
                        var fistLevel = Convert.ToUInt16(skillParams[1]);
                        if (fistLevel > 0)
                        {
                            Skills[SkillType.Fist] = new Skill(SkillType.Fist, fistLevel, 1.00, 1, fistLevel, (ushort)(fistLevel * 2));
                        }

                        break;
                    case MonsterSkill.AXEFIGHTING:
                        var axeLevel = Convert.ToUInt16(skillParams[1]);
                        if (axeLevel > 0)
                        {
                            Skills[SkillType.Fist] = new Skill(SkillType.Fist, axeLevel, 1.00, 1, axeLevel, (ushort)(axeLevel * 2));
                        }

                        break;
                    case MonsterSkill.SWORDFIGHTING:
                        var swordLevel = Convert.ToUInt16(skillParams[1]);
                        if (swordLevel > 0)
                        {
                            Skills[SkillType.Fist] = new Skill(SkillType.Fist, swordLevel, 1.00, 1, swordLevel, (ushort)(swordLevel * 2));
                        }

                        break;
                    case MonsterSkill.CLUBFIGHTING:
                        var clubLevel = Convert.ToUInt16(skillParams[1]);
                        if (clubLevel > 0)
                        {
                            Skills[SkillType.Fist] = new Skill(SkillType.Fist, clubLevel, 1.00, 1, clubLevel, (ushort)(clubLevel * 2));
                        }

                        break;
                }
            }
        }

        internal void SetInventory(string v)
        {
            if (Locked)
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
                    throw new InvalidDataException($"Unexpected number of elements in inventory line {v} on monster type {Name}.");
                }

                InventoryComposition.Add(new Tuple<ushort, byte, ushort>(Convert.ToUInt16(inventoryParams[0]), Convert.ToByte(inventoryParams[1]), Convert.ToUInt16(inventoryParams[2])));
            }
        }

        internal void SetPhrases(string v)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This MonsterType is locked and cannot be altered.");
            }

            Phrases.AddRange(CipParser.SplitByTokenPreserveQuoted(v, ','));
        }
    }
}