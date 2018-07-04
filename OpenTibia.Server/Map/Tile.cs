// <copyright file="Tile.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Items;
    using OpenTibia.Server.Parsing;

    public class Tile : ITile
    {
        private readonly Stack<uint> creatureIdsOnTile;

        private readonly Stack<IItem> topItems1OnTile;

        private readonly Stack<IItem> topItems2OnTile;

        private readonly Stack<IItem> downItemsOnTile;

        private byte[] cachedDescription;

        public Location Location { get; }

        public byte Flags { get; private set; }

        public IItem Ground { get; set; }

        public IEnumerable<uint> CreatureIds => this.creatureIdsOnTile;

        public IEnumerable<IItem> TopItems1 => this.topItems1OnTile;

        public IEnumerable<IItem> TopItems2 => this.topItems2OnTile;

        public IEnumerable<IItem> DownItems => this.downItemsOnTile;

        public bool HandlesCollision
        {
            get
            {
                return (this.Ground != null && this.Ground.HasCollision) || this.TopItems1.Any(i => i.HasCollision) || this.TopItems2.Any(i => i.HasCollision) || this.DownItems.Any(i => i.HasCollision);
            }
        }

        public IEnumerable<IItem> ItemsWithCollision
        {
            get
            {
                var items = new List<IItem>();

                if (this.Ground.HasCollision)
                {
                    items.Add(this.Ground);
                }

                items.AddRange(this.TopItems1.Where(i => i.HasCollision));
                items.AddRange(this.TopItems2.Where(i => i.HasCollision));
                items.AddRange(this.DownItems.Where(i => i.HasCollision));

                return items;
            }
        }

        public bool HandlesSeparation
        {
            get
            {
                return (this.Ground != null && this.Ground.HasSeparation) || this.TopItems1.Any(i => i.HasSeparation) || this.TopItems2.Any(i => i.HasSeparation) || this.DownItems.Any(i => i.HasSeparation);
            }
        }

        public IEnumerable<IItem> ItemsWithSeparation
        {
            get
            {
                var items = new List<IItem>();

                if (this.Ground.HasSeparation)
                {
                    items.Add(this.Ground);
                }

                items.AddRange(this.TopItems1.Where(i => i.HasSeparation));
                items.AddRange(this.TopItems2.Where(i => i.HasSeparation));
                items.AddRange(this.DownItems.Where(i => i.HasSeparation));

                return items;
            }
        }

        public bool IsHouse => false;

        public bool BlocksThrow
        {
            get
            {
                return (this.Ground != null && this.Ground.BlocksThrow) || this.TopItems1.Any(i => i.BlocksThrow) || this.TopItems2.Any(i => i.BlocksThrow) || this.DownItems.Any(i => i.BlocksThrow);
            }
        }

        public bool BlocksPass
        {
            get
            {
                return (this.Ground != null && this.Ground.BlocksPass) || this.CreatureIds.Any() || this.TopItems1.Any(i => i.BlocksPass) || this.TopItems2.Any(i => i.BlocksPass) || this.DownItems.Any(i => i.BlocksPass);
            }
        }

        public bool BlocksLay
        {
            get
            {
                return (this.Ground != null && this.Ground.BlocksLay) || this.TopItems1.Any(i => i.BlocksLay) || this.TopItems2.Any(i => i.BlocksLay) || this.DownItems.Any(i => i.BlocksLay);
            }
        }

        public byte[] CachedDescription
        {
            get
            {
                if (this.cachedDescription == null)
                {
                    this.cachedDescription = this.GetItemDescriptionBytes();
                }

                return this.cachedDescription;
            }
        }

        private byte[] GetItemDescriptionBytes()
        {
            // not valid to cache response if there are creatures.
            if (this.creatureIdsOnTile.Count > 0)
            {
                return null;
            }

            var tempBytes = new List<byte>();

            var count = 0;
            const int numberOfObjectsLimit = 9;

            if (this.Ground != null)
            {
                tempBytes.AddRange(BitConverter.GetBytes(this.Ground.Type.ClientId));
                count++;
            }

            foreach (var item in this.TopItems1)
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

            foreach (var item in this.TopItems2)
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

            foreach (var item in this.DownItems)
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

            return tempBytes.ToArray();
        }

        public bool CanBeWalked(byte avoidDamageType = 0)
        {
            return !this.CreatureIds.Any()
                && this.Ground != null
                && !this.Ground.IsPathBlocking(avoidDamageType)
                && !this.TopItems1.Any(i => i.IsPathBlocking(avoidDamageType))
                && !this.TopItems2.Any(i => i.IsPathBlocking(avoidDamageType))
                && !this.DownItems.Any(i => i.IsPathBlocking(avoidDamageType));
        }

        public bool HasThing(IThing thing, byte count = 1)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(count));
            }

            var creature = thing as Creature;
            var creaturesCheck = creature != null && this.creatureIdsOnTile.Contains(creature.CreatureId);

            var top1Check = thing is Item && this.topItems1OnTile.Count > 0 && this.topItems1OnTile.Peek() == thing && thing.Count >= count;
            var top2Check = thing is Item && this.topItems2OnTile.Count > 0 && this.topItems2OnTile.Peek() == thing && thing.Count >= count;
            var downCheck = thing is Item && this.downItemsOnTile.Count > 0 && this.downItemsOnTile.Peek() == thing && thing.Count >= count;

            return creaturesCheck || top1Check || top2Check || downCheck;
        }

        // public static HashSet<string> PropSet = new HashSet<string>();

        // public string LoadedFrom { get; set; }
        public Tile(ushort x, ushort y, sbyte z)
            : this(new Location { X = x, Y = y, Z = z })
        {
        }

        public Tile(Location loc)
        {
            this.Location = loc;
            this.creatureIdsOnTile = new Stack<uint>();
            this.topItems1OnTile = new Stack<IItem>();
            this.topItems2OnTile = new Stack<IItem>();
            this.downItemsOnTile = new Stack<IItem>();
        }

        public void AddThing(ref IThing thing, byte count)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            var creature = thing as Creature;
            var item = thing as Item;

            if (creature != null)
            {
                this.creatureIdsOnTile.Push(creature.CreatureId);
                creature.Tile = this;
                creature.Added();
                
                // invalidate the cache.
                this.cachedDescription = null;
            }
            else if (item != null)
            {
                if (item.IsGround)
                {
                    this.Ground = item;
                    item.Added();
                }
                else if (item.IsTop1)
                {
                    this.topItems1OnTile.Push(item);
                    item.Added();
                }
                else if (item.IsTop2)
                {
                    this.topItems2OnTile.Push(item);
                    item.Added();
                }
                else
                {
                    if (item.IsCumulative)
                    {
                        var currentItem = this.downItemsOnTile.Count > 0 ? this.downItemsOnTile.Peek() as Item : null;

                        if (currentItem != null && currentItem.Type == item.Type && currentItem.Amount < 100)
                        {
                            // add these up.
                            var remaining = currentItem.Amount + count;

                            var newCount = (byte)Math.Min(remaining, 100);

                            currentItem.Amount = newCount;

                            remaining -= newCount;

                            if (remaining > 0)
                            {
                                IThing newThing = ItemFactory.Create(item.Type.TypeId);
                                this.AddThing(ref newThing, (byte)remaining);
                                thing = newThing;
                            }
                        }
                        else
                        {
                            item.Amount = count;
                            this.downItemsOnTile.Push(item);
                            item.Added();
                        }
                    }
                    else
                    {
                        this.downItemsOnTile.Push(item);
                        item.Added();
                    }
                }

                item.Tile = this;

                // invalidate the cache.
                this.cachedDescription = null;
            }
        }

        public void RemoveThing(ref IThing thing, byte count)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            var creature = thing as Creature;
            var item = thing as Item;

            if (creature != null)
            {
                this.RemoveCreature(creature);
                creature.Tile = null;
                creature.Removed();
            }
            else if (item != null)
            {
                var removeItem = true;

                if (item.IsGround)
                {
                    this.Ground = null;
                    item.Removed();
                    removeItem = false;
                }
                else if (item.IsTop1)
                {
                    this.topItems1OnTile.Pop();
                    item.Removed();
                    removeItem = false;
                }
                else if (item.IsTop2)
                {
                    this.topItems2OnTile.Pop();
                    item.Removed();
                    removeItem = false;
                }
                else
                {
                    if (item.IsCumulative)
                    {
                        if (item.Amount < count) // throwing because this should have been checked before.
                        {
                            throw new ArgumentException("Remove count is greater than available.");
                        }

                        if (item.Amount > count)
                        {
                            // create a new item (it got split...)
                            var newItem = ItemFactory.Create(item.Type.TypeId);
                            newItem.SetAmount(count);
                            item.Amount -= count;

                            thing = newItem;
                            removeItem = false;
                        }
                    }
                }

                if (removeItem)
                {
                    this.downItemsOnTile.Pop();
                    item.Removed();
                    item.Tile = null;
                }
            }
            else
            {
                throw new InvalidCastException("Thing did not cast to either a CreatureId or Item.");
            }

            // invalidate the cache.
            this.cachedDescription = null;
        }

        public void RemoveCreature(ICreature c)
        {
            var tempStack = new Stack<uint>();
            ICreature removed = null;

            lock (this.creatureIdsOnTile)
            {
                while (removed == null && this.creatureIdsOnTile.Count > 0)
                {
                    var temp = this.creatureIdsOnTile.Pop();

                    if (c.CreatureId == temp)
                    {
                        removed = c;
                    }
                    else
                    {
                        tempStack.Push(temp);
                    }
                }

                while (tempStack.Count > 0)
                {
                    this.creatureIdsOnTile.Push(tempStack.Pop());
                }
            }

            // Console.WriteLine($"Removed creature {c.Name} at {this.Location}");
        }

        private void AddTopItem1(IItem i)
        {
            lock (this.topItems1OnTile)
            {
                this.topItems1OnTile.Push(i);

                // invalidate the cache.
                this.cachedDescription = null;
            }
        }

        private void AddTopItem2(IItem i)
        {
            lock (this.topItems2OnTile)
            {
                this.topItems2OnTile.Push(i);

                // invalidate the cache.
                this.cachedDescription = null;
            }
        }

        private void AddDownItem(IItem i)
        {
            lock (this.downItemsOnTile)
            {
                this.downItemsOnTile.Push(i);

                // invalidate the cache.
                this.cachedDescription = null;
            }
        }

        public void AddContent(object contentObj)
        {
            var content = contentObj as IEnumerable<CipElement>;

            if (content == null)
            {
                return;
            }

            var downItemStackToReverse = new Stack<IItem>();
            var top1ItemStackToReverse = new Stack<IItem>();
            var top2ItemStackToReverse = new Stack<IItem>();

            foreach (var element in content)
            {
                if (element.Data < 0)
                {
                    // this is a flag an is unexpected.
                    // TODO: proper logging.
                    if (!ServerConfiguration.SupressInvalidItemWarnings)
                    {
                        Console.WriteLine($"Tile.AddContent: Unexpected flag {element.Attributes?.First()?.Name}, igoring.");
                    }

                    continue;
                }

                try
                {
                    var item = ItemFactory.Create((ushort)element.Data);

                    if (item == null)
                    {
                        if (!ServerConfiguration.SupressInvalidItemWarnings)
                        {
                            Console.WriteLine($"Tile.AddContent: Item with id {element.Data} not found in the catalog, skipping.");
                        }

                        continue;
                    }

                    item.AddAttributes(element.Attributes);

                    if (item.IsGround)
                    {
                        this.Ground = item;
                    }
                    else if (item.IsTop1)
                    {
                        top1ItemStackToReverse.Push(item);
                    }
                    else if (item.IsTop2)
                    {
                        top2ItemStackToReverse.Push(item);
                    }
                    else
                    {
                        downItemStackToReverse.Push(item);
                    }

                    item.Tile = this;
                }
                catch (ArgumentException)
                {
                    // TODO: proper logging.
                    if (!ServerConfiguration.SupressInvalidItemWarnings)
                    {
                        Console.WriteLine($"Tile.AddContent: Invalid item {element.Data} at {this.Location}, skipping.");
                    }
                }
            }

            // Reverse and add the stacks.
            while (top1ItemStackToReverse.Count > 0)
            {
                this.AddTopItem1(top1ItemStackToReverse.Pop());
            }

            while (top2ItemStackToReverse.Count > 0)
            {
                this.AddTopItem2(top2ItemStackToReverse.Pop());
            }

            while (downItemStackToReverse.Count > 0)
            {
                this.AddDownItem(downItemStackToReverse.Pop());
            }
        }

        public IItem BruteFindItemWithId(ushort id)
        {
            if (this.Ground != null && this.Ground.ThingId == id)
            {
                return this.Ground;
            }

            foreach (var item in this.topItems1OnTile.Union(this.topItems2OnTile).Union(this.downItemsOnTile))
            {
                if (item.ThingId == id)
                {
                    return item;
                }
            }

            return null;
        }

        public IItem BruteRemoveItemWithId(ushort id)
        {
            if (this.Ground != null && this.Ground.ThingId == id)
            {
                var ground = this.Ground;

                this.Ground = null;

                return ground;
            }

            var downItemStackToReverse = new Stack<IItem>();
            var top1ItemStackToReverse = new Stack<IItem>();
            var top2ItemStackToReverse = new Stack<IItem>();

            var keepLooking = true;
            IItem itemFound = null;

            while (keepLooking && this.topItems1OnTile.Count > 0)
            {
                var item = this.topItems1OnTile.Pop();

                if (item.ThingId == id)
                {
                    itemFound = item;
                    keepLooking = false;
                    continue;
                }

                top1ItemStackToReverse.Push(item);
            }

            while (keepLooking && this.topItems2OnTile.Count > 0)
            {
                var item = this.topItems2OnTile.Pop();

                if (item.ThingId == id)
                {
                    itemFound = item;
                    keepLooking = false;
                    break;
                }

                top2ItemStackToReverse.Push(item);
            }

            while (keepLooking && this.downItemsOnTile.Count > 0)
            {
                var item = this.downItemsOnTile.Pop();

                if (item.ThingId == id)
                {
                    itemFound = item;
                    break;
                }

                downItemStackToReverse.Push(item);
            }

            // Reverse and add the stacks back
            while (top1ItemStackToReverse.Count > 0)
            {
                this.AddTopItem1(top1ItemStackToReverse.Pop());
            }

            while (top2ItemStackToReverse.Count > 0)
            {
                this.AddTopItem2(top2ItemStackToReverse.Pop());
            }

            while (downItemStackToReverse.Count > 0)
            {
                this.AddDownItem(downItemStackToReverse.Pop());
            }

            return itemFound;
        }

        public IThing GetThingAtStackPosition(byte stackPosition)
        {
            if (stackPosition == 0 && this.Ground != null)
            {
                return this.Ground;
            }

            var currentPos = this.Ground == null ? -1 : 0;

            if (stackPosition > currentPos + this.topItems1OnTile.Count)
            {
                currentPos += this.topItems1OnTile.Count;
            }
            else
            {
                foreach (var item in this.TopItems1)
                {
                    if (++currentPos == stackPosition)
                    {
                        return item;
                    }
                }
            }

            if (stackPosition > currentPos + this.topItems2OnTile.Count)
            {
                currentPos += this.topItems2OnTile.Count;
            }
            else
            {
                foreach (var item in this.TopItems2)
                {
                    if (++currentPos == stackPosition)
                    {
                        return item;
                    }
                }
            }

            if (stackPosition > currentPos + this.creatureIdsOnTile.Count)
            {
                currentPos += this.creatureIdsOnTile.Count;
            }
            else
            {
                foreach (var creatureId in this.CreatureIds)
                {
                    if (++currentPos == stackPosition)
                    {
                        return Game.Instance.GetCreatureWithId(creatureId);
                    }
                }
            }

            return stackPosition <= currentPos + this.downItemsOnTile.Count ? this.DownItems.FirstOrDefault(item => ++currentPos == stackPosition) : null;
        }

        public byte GetStackPosition(IThing thing)
        {
            if (thing == null)
            {
                throw new ArgumentNullException(nameof(thing));
            }

            if (this.Ground != null && thing == this.Ground)
            {
                return 0;
            }

            var n = 0;

            foreach (var item in this.TopItems1)
            {
                ++n;
                if (thing == item)
                {
                    return (byte)n;
                }
            }

            foreach (var item in this.TopItems2)
            {
                ++n;
                if (thing == item)
                {
                    return (byte)n;
                }
            }

            foreach (var creatureId in this.CreatureIds)
            {
                ++n;

                var creature = thing as ICreature;
                if (creature != null && creature.CreatureId == creatureId)
                {
                    return (byte)n;
                }
            }

            foreach (var item in this.DownItems)
            {
                ++n;
                if (thing == item)
                {
                    return (byte)n;
                }
            }

            // return byte.MaxValue; // TODO: throw?
            throw new Exception("Thing not found in tile.");
        }

        public void SetFlag(TileFlag flag)
        {
            this.Flags |= (byte)flag;
        }

        // public FloorChangeDirection FloorChange
        // {
        //    get
        //    {
        //        if (Ground.HasFlag(ItemFlag.FloorchangeDown))
        //        {
        //            return FloorChangeDirection.Down;
        //        }
        //        else
        //        {
        //            foreach (IItem item in TopItems1)
        //            {
        //                if (item.HasFlag(ItemFlag.TopOrder3))
        //                {
        //                    switch (item.Type)
        //                    {
        //                        case 1126:
        //                            return (FloorChangeDirection.Up | FloorChangeDirection.East);
        //                        case 1128:
        //                            return (FloorChangeDirection.Up | FloorChangeDirection.West);
        //                        case 1130:
        //                            return (FloorChangeDirection.Up | FloorChangeDirection.South);
        //                        default:
        //                        case 1132:
        //                            return (FloorChangeDirection.Up | FloorChangeDirection.North);
        //                    }
        //                }
        //            }
        //        }

        // return FloorChangeDirection.None;
        //    }
        // }

        // public bool IsWalkable { get { return Ground != null && !HasFlag(ItemFlag.Blocking); } }

        // public bool HasFlag(ItemFlag flagVal)
        // {
        //    if (Ground != null)
        //    {
        //        if (ItemReader.FindItem(Ground.Type).hasFlag(flagVal))
        //            return true;
        //    }

        // if (TopItems1.Count > 0)
        //    {
        //        foreach (IItem item in TopItems1)
        //        {
        //            if (ItemReader.FindItem(Ground.Type).hasFlag(flagVal))
        //                return true;
        //        }
        //    }

        // if (TopItems2.Count > 0)
        //    {
        //        foreach (IItem item in TopItems2)
        //        {
        //            if (ItemReader.FindItem(Ground.Type).hasFlag(flagVal))
        //                return true;
        //        }
        //    }

        // if (CreatureIds.Count > 0)
        //    {
        //        foreach (CreatureId creature in CreatureIds)
        //        {
        //            if (flagVal == ItemFlag.Blocking)
        //                return true;
        //        }
        //    }

        // if (DownItems.Count > 0)
        //    {
        //        foreach (IItem item in DownItems)
        //        {
        //            if (ItemReader.FindItem(Ground.Type).hasFlag(flagVal))
        //                return true;
        //        }
        //    }
        //    return false;
        // }
    }
}