// <copyright file="ItemType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Items
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using COMMO.Data.Contracts;
    using COMMO.Server.Data.Interfaces;

    public class ItemType : IItemType
    {
        public ushort TypeId { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public ISet<ItemFlag> Flags { get; }

        public IDictionary<ItemAttribute, IConvertible> DefaultAttributes { get; }

        public bool Locked { get; private set; }

        public ushort ClientId => Flags.Contains(ItemFlag.Disguise) ? Convert.ToUInt16(DefaultAttributes[ItemAttribute.DisguiseTarget]) : TypeId;

        public ItemType()
        {
            TypeId = 0;
            Name = string.Empty;
            Description = string.Empty;
            Flags = new HashSet<ItemFlag>();
            DefaultAttributes = new Dictionary<ItemAttribute, IConvertible>();
            Locked = false;
        }

        public void LockChanges()
        {
            Locked = true;
        }

        public void SetId(ushort typeId)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            TypeId = typeId;
        }

        public void SetName(string name)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            Name = name;
        }

        public void SetDescription(string description)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            Description = description.Trim('"');
        }

        public void SetFlag(ItemFlag flag)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            Flags.Add(flag);
        }

        public void SetAttribute(string attributeName, int attributeValue)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            ItemAttribute attribute;

            if (!Enum.TryParse(attributeName, out attribute))
            {
                throw new InvalidDataException($"Attempted to set an unknown Item attribute [{attributeName}].");
            }

            DefaultAttributes[attribute] = attributeValue;
        }
    }
}
