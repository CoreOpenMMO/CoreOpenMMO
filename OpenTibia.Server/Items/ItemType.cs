// <copyright file="ItemType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Items
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;

    public class ItemType : IItemType
    {
        public ushort TypeId { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public ISet<ItemFlag> Flags { get; }

        public IDictionary<ItemAttribute, IConvertible> DefaultAttributes { get; }

        public bool Locked { get; private set; }

        public ushort ClientId => this.Flags.Contains(ItemFlag.Disguise) ? Convert.ToUInt16(this.DefaultAttributes[ItemAttribute.DisguiseTarget]) : this.TypeId;

        public ItemType()
        {
            this.TypeId = 0;
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.Flags = new HashSet<ItemFlag>();
            this.DefaultAttributes = new Dictionary<ItemAttribute, IConvertible>();
            this.Locked = false;
        }

        public void LockChanges()
        {
            this.Locked = true;
        }

        public void SetId(ushort typeId)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            this.TypeId = typeId;
        }

        public void SetName(string name)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            this.Name = name;
        }

        public void SetDescription(string description)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            this.Description = description.Trim('"');
        }

        public void SetFlag(ItemFlag flag)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            this.Flags.Add(flag);
        }

        public void SetAttribute(string attributeName, int attributeValue)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            ItemAttribute attribute;

            if (!Enum.TryParse(attributeName, out attribute))
            {
                throw new InvalidDataException($"Attempted to set an unknown Item attribute [{attributeName}].");
            }

            this.DefaultAttributes[attribute] = attributeValue;
        }
    }
}
