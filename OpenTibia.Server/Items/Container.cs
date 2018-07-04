// <copyright file="Container.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Parsing;

    public class Container : Item, IContainer
    {
        public event OnContentAdded OnContentAdded;

        public event OnContentUpdated OnContentUpdated;

        public event OnContentRemoved OnContentRemoved;

        public IList<IItem> Content { get; }

        public Dictionary<uint, byte> OpenedBy { get; }

        private readonly object openedByLock;

        public byte Volume => Convert.ToByte(this.Attributes.ContainsKey(ItemAttribute.Capacity) ? this.Attributes[ItemAttribute.Capacity] : 0x08);

        public new Location Location => this.Parent?.Location ?? base.Location;

        public Container(ItemType type)
            : base(type)
        {
            this.Content = new List<IItem>();
            this.OpenedBy = new Dictionary<uint, byte>();
            this.openedByLock = new object();

            this.OnContentUpdated += Game.Instance.OnContainerContentUpdated;
            this.OnContentAdded += Game.Instance.OnContainerContentAdded;
            this.OnContentRemoved += Game.Instance.OnContainerContentRemoved;
        }

        // ~Container()
        // {
        //    OnContentUpdated -= Game.Instance.OnContainerContentUpdated;
        //    OnContentAdded -= Game.Instance.OnContainerContentAdded;
        //    OnContentRemoved -= Game.Instance.OnContainerContentRemoved;
        // }
        public override void AddContent(IEnumerable<object> contentObjs)
        {
            if (contentObjs == null)
            {
                throw new ArgumentNullException(nameof(contentObjs));
            }

            var content = contentObjs.Cast<CipElement>();

            foreach (var element in content)
            {
                if (element.Data < 0)
                {
                    // this is a flag an is unexpected.
                    // TODO: proper logging.
                    if (!ServerConfiguration.SupressInvalidItemWarnings)
                    {
                        Console.WriteLine($"Container.AddContent: Unexpected flag {element.Attributes?.First()?.Name}, ignoring.");
                    }

                    continue;
                }

                try
                {
                    var item = ItemFactory.Create((ushort)element.Data) as IItem;

                    if (item == null)
                    {
                        if (!ServerConfiguration.SupressInvalidItemWarnings)
                        {
                            Console.WriteLine($"Container.AddContent: Item with id {element.Data} not found in the catalog, skipping.");
                        }

                        continue;
                    }

                    // TODO: this is hacky.
                    ((Item)item).AddAttributes(element.Attributes);

                    this.AddContent(item, 0xFF);
                }
                catch (ArgumentException)
                {
                    // TODO: proper logging.
                    if (!ServerConfiguration.SupressInvalidItemWarnings)
                    {
                        Console.WriteLine($"Item.AddContent: Invalid item {element.Data} in item contents, skipping.");
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to add the joined item to this container's content at the default index.
        /// </summary>
        /// <param name="otherItem">The item to add.</param>
        /// <returns>True if the operation was successful, false otherwise</returns>
        public override bool Join(IItem otherItem)
        {
            if (this.Content.Count >= this.Volume)
            {
                // we can't add the additional item, since we're at capacity.
                return false;
            }

            otherItem.SetParent(this);
            this.Content.Add(otherItem);

            this.OnContentAdded?.Invoke(this, otherItem);

            return true;
        }

        public bool AddContent(IItem item, byte index)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            // Validate that the item being added is not a parent of this item.
            if (this.IsChildOf(item))
            {
                return false;
            }

            try
            {
                var existingItem = this.Content[this.Content.Count - index - 1];

                if (existingItem != null)
                {
                    var joinResult = existingItem.Join(item);

                    this.OnContentUpdated?.Invoke(this, index, existingItem);

                    if (joinResult)
                    {
                        return true;
                    }

                    // attempt to add to the parent of this item.
                    if (existingItem.Parent != null && existingItem.Parent.Join(item))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return this.Join(item);
        }

        public bool RemoveContent(ushort itemId, byte index, byte count, out IItem splitItem)
        {
            splitItem = null;

            // see if item at index is cummulative, and if it is the same type we're adding.
            IItem existingItem = null;

            try
            {
                existingItem = this.Content[this.Content.Count - index - 1];
            }
            catch
            {
                // ignored
            }

            if (existingItem == null || existingItem.Type.TypeId != itemId || existingItem.Count < count)
            {
                return false;
            }

            var separateResult = existingItem.Separate(count, out splitItem);

            if (separateResult)
            {
                if (existingItem.Amount == 0)
                {
                    existingItem.SetAmount(count); // restore the "removed" count, since we removed "all" of the item.

                    existingItem.SetParent(null);
                    this.Content.RemoveAt(this.Content.Count - index - 1);
                    this.OnContentRemoved?.Invoke(this, index);
                }
                else
                {
                    this.OnContentUpdated?.Invoke(this, index, existingItem);
                }
            }

            return separateResult;
        }

        /// <summary>
        /// Gets the Count of the item in the specified index of the <see cref="Content"/> of this container.
        /// </summary>
        /// <param name="fromIndex">The index to check at this container's <see cref="Container.Content"/>.</param>
        /// <param name="itemIdExpected">The id of the item to expect at that index.</param>
        /// <returns>Returns an <see cref="sbyte"/> with a value between 1 and 100 with the Count of the item at the index <paramref name="fromIndex"/>, 0 if the item does not match the type of the <paramref name="itemIdExpected"/>, or -1 if there is no item at that index.</returns>
        public sbyte CountContentAmountAt(byte fromIndex, ushort itemIdExpected = 0)
        {
            IItem existingItem = null;

            try
            {
                existingItem = this.Content[this.Content.Count - fromIndex - 1];
            }
            catch
            {
                // ignored
            }

            if (existingItem == null)
            {
                return -1;
            }

            if (existingItem.Type.TypeId != itemIdExpected)
            {
                return 0;
            }

            return (sbyte)Math.Min(existingItem.Count, (byte)100);
        }

        /// <summary>
        /// Adds the supplied creature id and container id to this container's tracking list of openers.
        /// </summary>
        /// <param name="creatureOpeningId">The id of the creature opening this container.</param>
        /// <param name="containerId">The id of this container in creature's view.</param>
        /// <returns>The containerId that this container knows for this creature.</returns>
        /// <remarks>The id returned may not match the one supplied if the container was already opened by this creature before.</remarks>
        public byte Open(uint creatureOpeningId, byte containerId)
        {
            lock (this.openedByLock)
            {
                if (!this.OpenedBy.ContainsKey(creatureOpeningId))
                {
                    this.OpenedBy.Add(creatureOpeningId, containerId);
                }

                return this.OpenedBy[creatureOpeningId];
            }
        }

        /// <summary>
        /// Removes the supplied <see cref="ICreature"/> from this container's tracking list of openers.
        /// </summary>
        /// <param name="creatureClosingId">The id of the creature closing this container.</param>
        public void Close(uint creatureClosingId)
        {
            lock (this.openedByLock)
            {
                if (this.OpenedBy.ContainsKey(creatureClosingId))
                {
                    this.OpenedBy.Remove(creatureClosingId);
                }
            }
        }

        /// <summary>
        /// Gets the current id known for this container to a supplied creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature to check for.</param>
        /// <returns>A non-negative number if an id was found, -1 otherwise.</returns>
        public sbyte GetIdFor(uint creatureId)
        {
            lock (this.openedByLock)
            {
                if (this.OpenedBy.ContainsKey(creatureId))
                {
                    return (sbyte)this.OpenedBy[creatureId];
                }
            }

            return -1;
        }

        private bool IsChildOf(IItem item)
        {
            var itemAsContainer = item as IContainer;

            while (itemAsContainer != null)
            {
                if (this == itemAsContainer)
                {
                    return true;
                }

                itemAsContainer = itemAsContainer.Parent;
            }

            return false;
        }
    }
}
