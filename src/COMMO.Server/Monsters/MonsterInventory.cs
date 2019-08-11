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
        private Dictionary<byte, Tuple<IItem, ushort>> _inventory;
        private byte _lastPosByte;

        private readonly object[] _recalcLocks;

        private byte _totalArmor;
        private byte _totalAttack;
        private byte _totalDefense;

        public ICreature Owner { get; }

        public MonsterInventory(ICreature owner, IEnumerable<Tuple<ushort, byte, ushort>> inventoryComposition, ushort maxCapacity = 100) // 100 is arbitrary.
        {
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _inventory = new Dictionary<byte, Tuple<IItem, ushort>>();

            _recalcLocks = new object[3];
            _lastPosByte = 0;

            _totalAttack = 0xFF;
            _totalArmor = 0xFF;
            _totalDefense = 0xFF;

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

        public IItem this[byte idx] => _inventory.ContainsKey(idx) ? _inventory[idx].Item1 : null;

        public byte TotalArmor
        {
            get
            {
                if (_totalArmor == 0xFF)
                {
                    lock (_recalcLocks[0])
                    {
                        if (_totalArmor == 0xFF)
                        {
                            var total = default(byte);

                            foreach (var tuple in _inventory.Values)
                            {
                                total += tuple.Item1.Armor;
                            }

                            _totalArmor = total;
                        }
                    }
                }

                return _totalArmor;
            }
        }

        public byte TotalAttack
        {
            get
            {
                if (_totalAttack != 0xFF)
                {
                    return _totalAttack;
                }

                lock (_recalcLocks[1])
                {
                    if (_totalAttack != 0xFF)
                    {
                        return _totalAttack;
                    }

                    var total = _inventory.Values.Aggregate(default(byte), (current, tuple) => Math.Max(current, tuple.Item1.Attack));

                    _totalAttack = total;
                }

                return _totalAttack;
            }
        }

        public byte TotalDefense
        {
            get
            {
                if (_totalDefense != 0xFF)
                {
                    return _totalDefense;
                }

                lock (_recalcLocks[2])
                {
                    if (_totalDefense != 0xFF)
                    {
                        return _totalDefense;
                    }

                    var total = _inventory.Values.Aggregate(default(byte), (current, tuple) => Math.Max(current, tuple.Item1.Defense));

                    _totalDefense = total;
                }

                return _totalDefense;
            }
        }

        public byte AttackRange => 0x01;

        public bool Add(IItem item, out IItem extraItem, byte positionByte = 0xFF, byte count = 1, ushort lossProbability = 300)
        {
            extraItem = null;

            _inventory[_lastPosByte++] = new Tuple<IItem, ushort>(item, lossProbability);

            return true;
        }

        public IItem Remove(byte positionByte, byte count, out bool wasPartial)
        {
            wasPartial = false;

            if (_inventory.ContainsKey(positionByte))
            {
                var found = _inventory[positionByte].Item1;

                if (found.Count < count)
                {
                    return null;
                }

                // remove the whole item
                if (found.Count == count)
                {
                    _inventory.Remove(positionByte);
                    found.SetHolder(null, default);

                    return found;
                }

                IItem newItem = ItemFactory.Create(found.Type.TypeId);

                newItem.SetAmount(count);
                found.SetAmount((byte)(found.Amount - count));

                wasPartial = true;
                return newItem;
            }

            _inventory = new Dictionary<byte, Tuple<IItem, ushort>>(_inventory);

            return null;
        }

        public IItem Remove(ushort itemId, byte count, out bool wasPartial)
        {
            wasPartial = false;
            bool removed = false;
            var slot = _inventory.Keys.FirstOrDefault(k => _inventory[k].Item1.Type.TypeId == itemId);

            try
            {
                var found = _inventory[slot].Item1;

                if (found.Count < count)
                {
                    return null;
                }

                // remove the whole item
                if (found.Count == count)
                {
                    _inventory.Remove(slot);
                    found.SetHolder(null, default);

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
                    _inventory = new Dictionary<byte, Tuple<IItem, ushort>>(_inventory);
                }
            }
        }
    }
}