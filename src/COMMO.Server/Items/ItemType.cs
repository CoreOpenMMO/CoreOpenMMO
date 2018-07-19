// <copyright file="ItemType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using COMMO.Data.Contracts;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Server.Items
{
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

        public void SetAttribute(ItemAttribute attribute, int attributeValue)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            DefaultAttributes[attribute] = attributeValue;
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

        public void ParseFlags(UInt32 flags)
        {
            if (HasFlag(flags, 1 << 0)) // blockSolid
                SetFlag(ItemFlag.CollisionEvent);

            if (HasFlag(flags, 1 << 1)) // blockProjectile
                SetFlag(ItemFlag.Unthrow);

            if (HasFlag(flags, 1 << 2)) // blockPathFind
                SetFlag(ItemFlag.Unpass);

            if (HasFlag(flags, 1 << 3)) // hasElevation
                SetFlag(ItemFlag.Height);

            if (HasFlag(flags, 1 << 4)) // isUsable
                SetFlag(ItemFlag.UseEvent);

            if (HasFlag(flags, 1 << 5)) // isPickupable
                SetFlag(ItemFlag.Take);

            if (HasFlag(flags, 1 << 6)) // isMoveable
                SetFlag(ItemFlag.Unmove);

            if (HasFlag(flags, 1 << 7)) // isStackable
                SetFlag(ItemFlag.Cumulative);

            //if (HasFlag(flags, 1 << 8)) // floorChangeDown -- unused

            //if (HasFlag(flags, 1 << 9)) // floorChangeNorth -- unused

            //if (HasFlag(flags, 1 << 10)) // floorChangeEast -- unused

            //if (HasFlag(flags, 1 << 11)) // floorChangeSouth -- unused

            //if (HasFlag(flags, 1 << 12)) // floorChangeWest -- unused

            if (HasFlag(flags, 1 << 13)) // alwaysOnTop
                SetFlag(ItemFlag.Top);

            if (HasFlag(flags, 1 << 14)) // isReadable
                SetFlag(ItemFlag.Text);

            if (HasFlag(flags, 1 << 15)) // isRotatable
                SetFlag(ItemFlag.Rotate);

            if (HasFlag(flags, 1 << 16)) // isHangable
                SetFlag(ItemFlag.Hang);

            if (HasFlag(flags, 1 << 17)) // isVertical
                SetFlag(ItemFlag.HookEast);

            if (HasFlag(flags, 1 << 18)) // isHorizontal
                SetFlag(ItemFlag.HookSouth);

            //if (HasFlag(flags, 1 << 19)) // cannotDecay -- unused

            if (HasFlag(flags, 1 << 20)) // allowDistRead
                SetFlag(ItemFlag.DistUse);

            //if (HasFlag(flags, 1 << 21)) // unused -- unused

            //if (HasFlag(flags, 1 << 22)) // isAnimation -- unused

            if (HasFlag(flags, 1 << 23)) // lookTrough
                SetFlag(ItemFlag.Top);
            else
                SetFlag(ItemFlag.Bottom);

            if (HasFlag(flags, 1 << 25)) // fullTile
                SetFlag(ItemFlag.Bank);

            if (HasFlag(flags, 1 << 26)) // forceUse
                SetFlag(ItemFlag.ForceUse);
        }

        private bool HasFlag(UInt32 flags, UInt32 flag)
        {
            return (flags & flag) != 0;
        }

    }
}
