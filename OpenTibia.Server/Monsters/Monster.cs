// <copyright file="Monster.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Monsters
{
    using System;
    using System.Linq;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;

    public class Monster : Creature
    {
        public MonsterType Type { get; }

        public uint Experience { get; }

        public sealed override IInventory Inventory { get; protected set; }

        public override bool CanBeMoved => !this.Type.Flags.Contains(CreatureFlag.Unpushable);

        public override ushort AttackPower => Math.Max(this.Type.Attack, this.Inventory.TotalAttack);

        public override ushort ArmorRating => Math.Max(this.Type.Armor, this.Inventory.TotalArmor);

        public override ushort DefensePower => Math.Max(this.Type.Defense, this.Inventory.TotalDefense);

        public override byte AutoAttackRange => (byte)(this.Type.Flags.Contains(CreatureFlag.DistanceFighting) ? 5 : 1);

        public Monster(MonsterType monsterType)
            : base(GetNewId(), monsterType.Name, monsterType.Article, monsterType.MaxHitPoints, monsterType.MaxManaPoints, monsterType.Corpse)
        {
            this.Type = monsterType;
            this.Experience = monsterType.Experience;
            this.Speed += monsterType.Speed;
            this.Outfit = monsterType.Outfit;

            this.Inventory = new MonsterInventory(this, monsterType.InventoryComposition);

            foreach (var kvp in this.Type.Skills.ToList())
            {
                this.Type.Skills[kvp.Key] = kvp.Value;
            }
        }
    }
}
