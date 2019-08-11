// <copyright file="MonsterInventory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;
using COMMO.Server.Items;

namespace COMMO.Server.Monsters
{
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
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            inventory = new Dictionary<byte, Tuple<IItem, ushort>>();

            recalcLocks = new object[3];
            lastPosByte = 0;

            totalAttack = 0xFF;
            totalArmor = 0xFF;
            totalDefense = 0xFF;

            DetermineLoot(inventoryComposition, maxCapacity);
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

				if (!(ItemFactory.Create(tuple.Item1) is IItem newItem)) {
					Console.WriteLine($"Unknown item with id {tuple.Item1} as loot in monster type {(Owner as Monster)?.Type.RaceId}.");
					continue;
				}

				if (newItem.IsCumulative)
                {
                    var amount = (byte)(rng.Next(tuple.Item2) + 1);

                    newItem.SetAmount(amount);
                }

				Add(newItem, out var unused);
			}
        }

        public IItem this[byte idx] => inventory.ContainsKey(idx) ? inventory[idx].Item1 : null;

        public byte TotalArmor
        {
            get
            {
                if (totalArmor == 0xFF)
                {
                    lock (recalcLocks[0])
                    {
                        if (totalArmor == 0xFF)
                        {
                            var total = default(byte);

                            foreach (var tuple in inventory.Values)
                            {
                                total += tuple.Item1.Armor;
                            }

                            totalArmor = total;
                        }
                    }
                }

                return totalArmor;
            }
        }

        public byte TotalAttack
        {
            get
            {
                if (totalAttack != 0xFF)
                {
                    return totalAttack;
                }

                lock (recalcLocks[1])
                {
                    if (totalAttack != 0xFF)
                    {
                        return totalAttack;
                    }

                    var total = inventory.Values.Aggregate(default(byte), (current, tuple) => Math.Max(current, tuple.Item1.Attack));

                    totalAttack = total;
                }

                return totalAttack;
            }
        }

        public byte TotalDefense
        {
            get
            {
                if (totalDefense != 0xFF)
                {
                    return totalDefense;
                }

                lock (recalcLocks[2])
                {
                    if (totalDefense != 0xFF)
                    {
                        return totalDefense;
                    }

                    var total = inventory.Values.Aggregate(default(byte), (current, tuple) => Math.Max(current, tuple.Item1.Defense));

                    totalDefense = total;
                }

                return totalDefense;
            }
        }

        public byte AttackRange => 0x01;

        public bool Add(IItem item, out IItem extraItem, byte positionByte = 0xFF, byte count = 1, ushort lossProbability = 300)
        {
            extraItem = null;

            inventory[lastPosByte++] = new Tuple<IItem, ushort>(item, lossProbability);

            return true;
        }

        public IItem Remove(byte positionByte, byte count, out bool wasPartial)
        {
            wasPartial = false;

            if (inventory.ContainsKey(positionByte))
            {
                var found = inventory[positionByte].Item1;

                if (found.Count < count)
                {
                    return null;
                }

                // remove the whole item
                if (found.Count == count)
                {
                    inventory.Remove(positionByte);
                    found.SetHolder(null, default(Location));

                    return found;
                }

                IItem newItem = ItemFactory.Create(found.Type.TypeId);

                newItem.SetAmount(count);
                found.SetAmount((byte)(found.Amount - count));

                wasPartial = true;
                return newItem;
            }

            inventory = new Dictionary<byte, Tuple<IItem, ushort>>(inventory);

            return null;
        }

        public IItem Remove(ushort itemId, byte count, out bool wasPartial)
        {
            wasPartial = false;
            bool removed = false;
            var slot = inventory.Keys.FirstOrDefault(k => inventory[k].Item1.Type.TypeId == itemId);

            try
            {
                var found = inventory[slot].Item1;

                if (found.Count < count)
                {
                    return null;
                }

                // remove the whole item
                if (found.Count == count)
                {
                    inventory.Remove(slot);
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
                    inventory = new Dictionary<byte, Tuple<IItem, ushort>>(inventory);
                }
            }
        }
    }
}