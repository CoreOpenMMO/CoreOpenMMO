// <copyright file="Tile.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using COMMO.Data.Contracts;
    using COMMO.Server.Data.Interfaces;
    using COMMO.Server.Data.Models.Structs;
    using COMMO.Server.Items;
    using COMMO.Server.Parsing;

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

        public IEnumerable<uint> CreatureIds => creatureIdsOnTile;

        public IEnumerable<IItem> TopItems1 => topItems1OnTile;

        public IEnumerable<IItem> TopItems2 => topItems2OnTile;

        public IEnumerable<IItem> DownItems => downItemsOnTile;

        public bool HandlesCollision
        {
            get
            {
                return (Ground != null && Ground.HasCollision) || TopItems1.Any(i => i.HasCollision) || TopItems2.Any(i => i.HasCollision) || DownItems.Any(i => i.HasCollision);
            }
        }

        public IEnumerable<IItem> ItemsWithCollision
        {
            get
            {
                var items = new List<IItem>();

                if (Ground.HasCollision)
                {
                    items.Add(Ground);
                }

                items.AddRange(TopItems1.Where(i => i.HasCollision));
                items.AddRange(TopItems2.Where(i => i.HasCollision));
                items.AddRange(DownItems.Where(i => i.HasCollision));

                return items;
            }
        }

        public bool HandlesSeparation
        {
            get
            {
                return (Ground != null && Ground.HasSeparation) || TopItems1.Any(i => i.HasSeparation) || TopItems2.Any(i => i.HasSeparation) || DownItems.Any(i => i.HasSeparation);
            }
        }

        public IEnumerable<IItem> ItemsWithSeparation
        {
            get
            {
                var items = new List<IItem>();

                if (Ground.HasSeparation)
                {
                    items.Add(Ground);
                }

                items.AddRange(TopItems1.Where(i => i.HasSeparation));
                items.AddRange(TopItems2.Where(i => i.HasSeparation));
                items.AddRange(DownItems.Where(i => i.HasSeparation));

                return items;
            }
        }

        public bool IsHouse => false;

        public bool BlocksThrow
        {
            get
            {
                return (Ground != null && Ground.BlocksThrow) || TopItems1.Any(i => i.BlocksThrow) || TopItems2.Any(i => i.BlocksThrow) || DownItems.Any(i => i.BlocksThrow);
            }
        }

        public bool BlocksPass
        {
            get
            {
                return (Ground != null && Ground.BlocksPass) || CreatureIds.Any() || TopItems1.Any(i => i.BlocksPass) || TopItems2.Any(i => i.BlocksPass) || DownItems.Any(i => i.BlocksPass);
            }
        }

        public bool BlocksLay
        {
            get
            {
                return (Ground != null && Ground.BlocksLay) || TopItems1.Any(i => i.BlocksLay) || TopItems2.Any(i => i.BlocksLay) || DownItems.Any(i => i.BlocksLay);
            }
        }

        public byte[] CachedDescription
        {
            get
            {
                if (cachedDescription == null)
                {
                    cachedDescription = GetItemDescriptionBytes();
                }

                return cachedDescription;
            }
        }

        private byte[] GetItemDescriptionBytes()
        {
            // not valid to cache response if there are creatures.
            if (creatureIdsOnTile.Count > 0)
            {
                return null;
            }

            var tempBytes = new List<byte>();

            var count = 0;
            const int numberOfObjectsLimit = 9;

            if (Ground != null)
            {
                tempBytes.AddRange(BitConverter.GetBytes(Ground.Type.ClientId));
                count++;
            }

            foreach (var item in TopItems1)
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

            foreach (var item in TopItems2)
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

            foreach (var item in DownItems)
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
            return !CreatureIds.Any()
                && Ground != null
                && !Ground.IsPathBlocking(avoidDamageType)
                && !TopItems1.Any(i => i.IsPathBlocking(avoidDamageType))
                && !TopItems2.Any(i => i.IsPathBlocking(avoidDamageType))
                && !DownItems.Any(i => i.IsPathBlocking(avoidDamageType));
        }

        public bool HasThing(IThing thing, byte count = 1)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(count));
            }

            var creature = thing as Creature;
            var creaturesCheck = creature != null && creatureIdsOnTile.Contains(creature.CreatureId);

            var top1Check = thing is Item && topItems1OnTile.Count > 0 && topItems1OnTile.Peek() == thing && thing.Count >= count;
            var top2Check = thing is Item && topItems2OnTile.Count > 0 && topItems2OnTile.Peek() == thing && thing.Count >= count;
            var downCheck = thing is Item && downItemsOnTile.Count > 0 && downItemsOnTile.Peek() == thing && thing.Count >= count;

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
            Location = loc;
            creatureIdsOnTile = new Stack<uint>();
            topItems1OnTile = new Stack<IItem>();
            topItems2OnTile = new Stack<IItem>();
            downItemsOnTile = new Stack<IItem>();
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
                creatureIdsOnTile.Push(creature.CreatureId);
                creature.Tile = this;
                creature.Added();
                
                // invalidate the cache.
                cachedDescription = null;
            }
            else if (item != null)
            {
                if (item.IsGround)
                {
                    Ground = item;
                    item.Added();
                }
                else if (item.IsTop1)
                {
                    topItems1OnTile.Push(item);
                    item.Added();
                }
                else if (item.IsTop2)
                {
                    topItems2OnTile.Push(item);
                    item.Added();
                }
                else
                {
                    if (item.IsCumulative)
                    {
                        var currentItem = downItemsOnTile.Count > 0 ? downItemsOnTile.Peek() as Item : null;

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
                                AddThing(ref newThing, (byte)remaining);
                                thing = newThing;
                            }
                        }
                        else
                        {
                            item.Amount = count;
                            downItemsOnTile.Push(item);
                            item.Added();
                        }
                    }
                    else
                    {
                        downItemsOnTile.Push(item);
                        item.Added();
                    }
                }

                item.Tile = this;

                // invalidate the cache.
                cachedDescription = null;
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
                RemoveCreature(creature);
                creature.Tile = null;
                creature.Removed();
            }
            else if (item != null)
            {
                var removeItem = true;

                if (item.IsGround)
                {
                    Ground = null;
                    item.Removed();
                    removeItem = false;
                }
                else if (item.IsTop1)
                {
                    topItems1OnTile.Pop();
                    item.Removed();
                    removeItem = false;
                }
                else if (item.IsTop2)
                {
                    topItems2OnTile.Pop();
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
                    downItemsOnTile.Pop();
                    item.Removed();
                    item.Tile = null;
                }
            }
            else
            {
                throw new InvalidCastException("Thing did not cast to either a CreatureId or Item.");
            }

            // invalidate the cache.
            cachedDescription = null;
        }

        public void RemoveCreature(ICreature c)
        {
            var tempStack = new Stack<uint>();
            ICreature removed = null;

            lock (creatureIdsOnTile)
            {
                while (removed == null && creatureIdsOnTile.Count > 0)
                {
                    var temp = creatureIdsOnTile.Pop();

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
                    creatureIdsOnTile.Push(tempStack.Pop());
                }
            }

            // Console.WriteLine($"Removed creature {c.Name} at {Location}");
        }

        private void AddTopItem1(IItem i)
        {
            lock (topItems1OnTile)
            {
                topItems1OnTile.Push(i);

                // invalidate the cache.
                cachedDescription = null;
            }
        }

        private void AddTopItem2(IItem i)
        {
            lock (topItems2OnTile)
            {
                topItems2OnTile.Push(i);

                // invalidate the cache.
                cachedDescription = null;
            }
        }

        private void AddDownItem(IItem i)
        {
            lock (downItemsOnTile)
            {
                downItemsOnTile.Push(i);

                // invalidate the cache.
                cachedDescription = null;
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
                        Ground = item;
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
                        Console.WriteLine($"Tile.AddContent: Invalid item {element.Data} at {Location}, skipping.");
                    }
                }
            }

            // Reverse and add the stacks.
            while (top1ItemStackToReverse.Count > 0)
            {
                AddTopItem1(top1ItemStackToReverse.Pop());
            }

            while (top2ItemStackToReverse.Count > 0)
            {
                AddTopItem2(top2ItemStackToReverse.Pop());
            }

            while (downItemStackToReverse.Count > 0)
            {
                AddDownItem(downItemStackToReverse.Pop());
            }
        }

        public IItem BruteFindItemWithId(ushort id)
        {
            if (Ground != null && Ground.ThingId == id)
            {
                return Ground;
            }

            foreach (var item in topItems1OnTile.Union(topItems2OnTile).Union(downItemsOnTile))
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
            if (Ground != null && Ground.ThingId == id)
            {
                var ground = Ground;

                Ground = null;

                return ground;
            }

            var downItemStackToReverse = new Stack<IItem>();
            var top1ItemStackToReverse = new Stack<IItem>();
            var top2ItemStackToReverse = new Stack<IItem>();

            var keepLooking = true;
            IItem itemFound = null;

            while (keepLooking && topItems1OnTile.Count > 0)
            {
                var item = topItems1OnTile.Pop();

                if (item.ThingId == id)
                {
                    itemFound = item;
                    keepLooking = false;
                    continue;
                }

                top1ItemStackToReverse.Push(item);
            }

            while (keepLooking && topItems2OnTile.Count > 0)
            {
                var item = topItems2OnTile.Pop();

                if (item.ThingId == id)
                {
                    itemFound = item;
                    keepLooking = false;
                    break;
                }

                top2ItemStackToReverse.Push(item);
            }

            while (keepLooking && downItemsOnTile.Count > 0)
            {
                var item = downItemsOnTile.Pop();

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
                AddTopItem1(top1ItemStackToReverse.Pop());
            }

            while (top2ItemStackToReverse.Count > 0)
            {
                AddTopItem2(top2ItemStackToReverse.Pop());
            }

            while (downItemStackToReverse.Count > 0)
            {
                AddDownItem(downItemStackToReverse.Pop());
            }

            return itemFound;
        }

        public IThing GetThingAtStackPosition(byte stackPosition)
        {
            if (stackPosition == 0 && Ground != null)
            {
                return Ground;
            }

            var currentPos = Ground == null ? -1 : 0;

            if (stackPosition > currentPos + topItems1OnTile.Count)
            {
                currentPos += topItems1OnTile.Count;
            }
            else
            {
                foreach (var item in TopItems1)
                {
                    if (++currentPos == stackPosition)
                    {
                        return item;
                    }
                }
            }

            if (stackPosition > currentPos + topItems2OnTile.Count)
            {
                currentPos += topItems2OnTile.Count;
            }
            else
            {
                foreach (var item in TopItems2)
                {
                    if (++currentPos == stackPosition)
                    {
                        return item;
                    }
                }
            }

            if (stackPosition > currentPos + creatureIdsOnTile.Count)
            {
                currentPos += creatureIdsOnTile.Count;
            }
            else
            {
                foreach (var creatureId in CreatureIds)
                {
                    if (++currentPos == stackPosition)
                    {
                        return Game.Instance.GetCreatureWithId(creatureId);
                    }
                }
            }

            return stackPosition <= currentPos + downItemsOnTile.Count ? DownItems.FirstOrDefault(item => ++currentPos == stackPosition) : null;
        }

        public byte GetStackPosition(IThing thing)
        {
            if (thing == null)
            {
                throw new ArgumentNullException(nameof(thing));
            }

            if (Ground != null && thing == Ground)
            {
                return 0;
            }

            var n = 0;

            foreach (var item in TopItems1)
            {
                ++n;
                if (thing == item)
                {
                    return (byte)n;
                }
            }

            foreach (var item in TopItems2)
            {
                ++n;
                if (thing == item)
                {
                    return (byte)n;
                }
            }

            foreach (var creatureId in CreatureIds)
            {
                ++n;

                var creature = thing as ICreature;
                if (creature != null && creature.CreatureId == creatureId)
                {
                    return (byte)n;
                }
            }

            foreach (var item in DownItems)
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
            Flags |= (byte)flag;
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