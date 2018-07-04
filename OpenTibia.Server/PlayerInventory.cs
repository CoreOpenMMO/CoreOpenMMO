// <copyright file="PlayerInventory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Items;
    using OpenTibia.Server.Notifications;

    // public delegate void OnSetInventoryItem(Slot slot, IItem item);
    // public delegate void OnUnsetInventoryItem(Slot slot);
    internal class PlayerInventory : IInventory
    {
        private Dictionary<Slot, Tuple<IItem, ushort>> Inventory { get; }

        public byte TotalAttack => (byte)Math.Max(this.Inventory.ContainsKey(Slot.Left) ? this.Inventory[Slot.Left].Item1.Attack : 0, this.Inventory.ContainsKey(Slot.Right) ? this.Inventory[Slot.Right].Item1.Attack : 0);

        public byte TotalDefense => (byte)Math.Max(this.Inventory.ContainsKey(Slot.Left) ? this.Inventory[Slot.Left].Item1.Defense : 0, this.Inventory.ContainsKey(Slot.Right) ? this.Inventory[Slot.Right].Item1.Defense : 0);

        public byte TotalArmor
        {
            get
            {
                byte totalArmor = 0;

                totalArmor += (byte)(this.Inventory.ContainsKey(Slot.Necklace) ? this.Inventory[Slot.Necklace].Item1.Armor : 0);
                totalArmor += (byte)(this.Inventory.ContainsKey(Slot.Head) ? this.Inventory[Slot.Head].Item1.Armor : 0);
                totalArmor += (byte)(this.Inventory.ContainsKey(Slot.Body) ? this.Inventory[Slot.Body].Item1.Armor : 0);
                totalArmor += (byte)(this.Inventory.ContainsKey(Slot.Legs) ? this.Inventory[Slot.Legs].Item1.Armor : 0);
                totalArmor += (byte)(this.Inventory.ContainsKey(Slot.Feet) ? this.Inventory[Slot.Feet].Item1.Armor : 0);
                totalArmor += (byte)(this.Inventory.ContainsKey(Slot.Ring) ? this.Inventory[Slot.Ring].Item1.Armor : 0);

                return totalArmor;
            }
        }

        public byte AttackRange => (byte)Math.Max(
            Math.Max(
            this.Inventory.ContainsKey(Slot.Left) ? this.Inventory[Slot.Left].Item1.Range : 0,
                this.Inventory.ContainsKey(Slot.Right) ? this.Inventory[Slot.Right].Item1.Range : 0),
            this.Inventory.ContainsKey(Slot.TwoHanded) ? this.Inventory[Slot.TwoHanded].Item1.Range : 0);

        public ICreature Owner { get; }

        public PlayerInventory(ICreature owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            this.Owner = owner;
            this.Inventory = new Dictionary<Slot, Tuple<IItem, ushort>>();
        }

        public IItem this[byte slot] => !this.Inventory.ContainsKey((Slot)slot) ? null : this.Inventory[(Slot)slot].Item1;

        public bool Add(IItem item, out IItem extraItem, byte positionByte, byte count = 1, ushort lossProbability = 300)
        {
            if (count == 0 || count > 100)
            {
                throw new ArgumentException($"Invalid count {count}.", nameof(count));
            }

            extraItem = null;

            if (item == null || positionByte > (byte)Slot.WhereEver)
            {
                return false;
            }

            var targetSlot = (Slot)positionByte;

            // TODO: check dress positions here.

            // if (targetSlot != Slot.Right && targetSlot != Slot.Left && targetSlot != Slot.WhereEver)
            // {

            // }
            try
            {
                var current = this.Inventory[targetSlot];

                if (current != null)
                {
                    var joinResult = current.Item1.Join(item);

                    // update the added item in the slot.
                    Game.Instance.NotifySinglePlayer(this.Owner as IPlayer, conn => new GenericNotification(conn, new InventorySetSlotPacket { Slot = targetSlot, Item = current.Item1 }));

                    if (joinResult || current.Item1.IsContainer)
                    {
                        return joinResult;
                    }

                    // exchange items
                    if (current.Item1.IsCumulative && item.Type.TypeId == current.Item1.Type.TypeId && current.Item1.Count == 100)
                    {
                        extraItem = item;
                        item = current.Item1;
                    }
                    else
                    {
                        extraItem = current.Item1;
                        extraItem.SetHolder(null, default(Location));
                    }
                }
            }
            catch
            {
                // ignored
            }

            // set the item in place.
            this.Inventory[targetSlot] = new Tuple<IItem, ushort>(item, item.IsContainer ? (ushort)1000 : lossProbability);

            item.SetHolder(this.Owner, new Location { X = 0xFFFF, Y = 0, Z = (sbyte)targetSlot });

            // update the added item in the slot.
            Game.Instance.NotifySinglePlayer(this.Owner as IPlayer, conn => new GenericNotification(conn, new InventorySetSlotPacket { Slot = targetSlot, Item = item }));

            return true;
        }

        public IItem Remove(byte positionByte, byte count, out bool wasPartial)
        {
            wasPartial = false;

            if (positionByte == (byte)Slot.TwoHanded || positionByte == (byte)Slot.WhereEver)
            {
                return null;
            }

            if (this.Inventory.ContainsKey((Slot)positionByte))
            {
                var found = this.Inventory[(Slot)positionByte].Item1;

                if (found.Count < count)
                {
                    return null;
                }

                // remove the whole item
                if (found.Count == count)
                {
                    this.Inventory.Remove((Slot)positionByte);
                    found.SetHolder(null, default(Location));

                    // update the slot.
                    Game.Instance.NotifySinglePlayer(
                        this.Owner as IPlayer,
                        conn => new GenericNotification(
                            conn,
                            new InventoryClearSlotPacket { Slot = (Slot)positionByte }));

                    return found;
                }

                IItem newItem = ItemFactory.Create(found.Type.TypeId);

                newItem.SetAmount(count);
                found.SetAmount((byte)(found.Amount - count));

                // update the remaining item in the slot.
                Game.Instance.NotifySinglePlayer(
                    this.Owner as IPlayer,
                    conn => new GenericNotification(
                        conn,
                        new InventorySetSlotPacket { Slot = (Slot)positionByte, Item = found }));

                wasPartial = true;
                return newItem;
            }

            return null;
        }

        public IItem Remove(ushort itemId, byte count, out bool wasPartial)
        {
            wasPartial = false;

            var slot = this.Inventory.Keys.FirstOrDefault(k => this.Inventory[k].Item1.Type.TypeId == itemId);

            if (slot != default(Slot))
            {
                var found = this.Inventory[slot].Item1;

                if (found.Count < count)
                {
                    return null;
                }

                // remove the whole item
                if (found.Count == count)
                {
                    this.Inventory.Remove(slot);
                    found.SetHolder(null, default(Location));

                    // update the slot.
                    Game.Instance.NotifySinglePlayer(
                        this.Owner as IPlayer,
                        conn => new GenericNotification(
                            conn,
                            new InventoryClearSlotPacket { Slot = slot }));

                    return found;
                }

                IItem newItem = ItemFactory.Create(found.Type.TypeId);

                newItem.SetAmount(count);
                found.SetAmount((byte)(found.Amount - count));

                // update the remaining item in the slot.
                Game.Instance.NotifySinglePlayer(
                    this.Owner as IPlayer,
                    conn => new GenericNotification(
                        conn,
                        new InventorySetSlotPacket { Slot = slot, Item = found }));

                wasPartial = true;
                return newItem;
            }

            // TODO: exhaustive search of container items here.
            return null;
        }
    }
}
