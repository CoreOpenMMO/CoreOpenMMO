// <copyright file="MonsterInventory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Monsters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Items;

    internal class MonsterInventory : IInventory
    {
        private Dictionary<byte, Tuple<IItem, ushort>> inventory;
        private byte lastPosByte;

        private readonly object[] recalcLocks;

        private byte totalArmor;
        private byte totalAttack;
        private byte totalDefense;

        public ICreature Owner { get; }

        public MonsterInventory(ICreature owner, IEnumerable<Tuple<ushort, byte, ushort>> inventoryComposition, ushort maxCapacity = 100) // 100 is arbitrary.
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            this.Owner = owner;
            this.inventory = new Dictionary<byte, Tuple<IItem, ushort>>();

            this.recalcLocks = new object[3];
            this.lastPosByte = 0;

            this.totalAttack = 0xFF;
            this.totalArmor = 0xFF;
            this.totalDefense = 0xFF;

            this.DetermineLoot(inventoryComposition, maxCapacity);
        }

        private void DetermineLoot(IEnumerable<Tuple<ushort, byte, ushort>> inventoryComposition, ushort maxCapacity)
        {
            var rng = new Random();

            foreach (var tuple in inventoryComposition)
            {
                if (rng.Next(1000) > tuple.Item3)
                {
                    continue;
                }

                // got lucky!
                var newItem = ItemFactory.Create(tuple.Item1) as IItem;

                if (newItem == null)
                {
                    Console.WriteLine($"Unknown item with id {tuple.Item1} as loot in monster type {(this.Owner as Monster)?.Type.RaceId}.");
                    continue;
                }

                if (newItem.IsCumulative)
                {
                    var amount = (byte)(rng.Next(tuple.Item2) + 1);

                    newItem.SetAmount(amount);
                }

                IItem unused;
                this.Add(newItem, out unused);
            }
        }

        public IItem this[byte idx] => this.inventory.ContainsKey(idx) ? this.inventory[idx].Item1 : null;

        public byte TotalArmor
        {
            get
            {
                if (this.totalArmor == 0xFF)
                {
                    lock (this.recalcLocks[0])
                    {
                        if (this.totalArmor == 0xFF)
                        {
                            var total = default(byte);

                            foreach (var tuple in this.inventory.Values)
                            {
                                total += tuple.Item1.Armor;
                            }

                            this.totalArmor = total;
                        }
                    }
                }

                return this.totalArmor;
            }
        }

        public byte TotalAttack
        {
            get
            {
                if (this.totalAttack != 0xFF)
                {
                    return this.totalAttack;
                }

                lock (this.recalcLocks[1])
                {
                    if (this.totalAttack != 0xFF)
                    {
                        return this.totalAttack;
                    }

                    var total = this.inventory.Values.Aggregate(default(byte), (current, tuple) => Math.Max(current, tuple.Item1.Attack));

                    this.totalAttack = total;
                }

                return this.totalAttack;
            }
        }

        public byte TotalDefense
        {
            get
            {
                if (this.totalDefense != 0xFF)
                {
                    return this.totalDefense;
                }

                lock (this.recalcLocks[2])
                {
                    if (this.totalDefense != 0xFF)
                    {
                        return this.totalDefense;
                    }

                    var total = this.inventory.Values.Aggregate(default(byte), (current, tuple) => Math.Max(current, tuple.Item1.Defense));

                    this.totalDefense = total;
                }

                return this.totalDefense;
            }
        }

        public byte AttackRange => 0x01;

        public bool Add(IItem item, out IItem extraItem, byte positionByte = 0xFF, byte count = 1, ushort lossProbability = 300)
        {
            extraItem = null;

            this.inventory[this.lastPosByte++] = new Tuple<IItem, ushort>(item, lossProbability);

            return true;
        }

        public IItem Remove(byte positionByte, byte count, out bool wasPartial)
        {
            wasPartial = false;

            if (this.inventory.ContainsKey(positionByte))
            {
                var found = this.inventory[positionByte].Item1;

                if (found.Count < count)
                {
                    return null;
                }

                // remove the whole item
                if (found.Count == count)
                {
                    this.inventory.Remove(positionByte);
                    found.SetHolder(null, default(Location));

                    return found;
                }

                IItem newItem = ItemFactory.Create(found.Type.TypeId);

                newItem.SetAmount(count);
                found.SetAmount((byte)(found.Amount - count));

                wasPartial = true;
                return newItem;
            }

            this.inventory = new Dictionary<byte, Tuple<IItem, ushort>>(this.inventory);

            return null;
        }

        public IItem Remove(ushort itemId, byte count, out bool wasPartial)
        {
            wasPartial = false;
            bool removed = false;
            var slot = this.inventory.Keys.FirstOrDefault(k => this.inventory[k].Item1.Type.TypeId == itemId);

            try
            {
                var found = this.inventory[slot].Item1;

                if (found.Count < count)
                {
                    return null;
                }

                // remove the whole item
                if (found.Count == count)
                {
                    this.inventory.Remove(slot);
                    found.SetHolder(null, default(Location));

                    removed = true;
                    return found;
                }

                IItem newItem = ItemFactory.Create(found.Type.TypeId);

                newItem.SetAmount(count);
                found.SetAmount((byte)(found.Amount - count));

                wasPartial = true;
                removed = true;
                return newItem;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (removed)
                {
                    this.inventory = new Dictionary<byte, Tuple<IItem, ushort>>(this.inventory);
                }
            }
        }
    }
}