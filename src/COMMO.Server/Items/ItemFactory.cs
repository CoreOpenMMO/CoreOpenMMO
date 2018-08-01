// <copyright file="ItemFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using COMMO.Data.Contracts;

namespace COMMO.Server.Items {
	public static class ItemFactory
    {
        public static object InitLock = new object();

        public static Dictionary<ushort, ItemType> ItemsCatalog { get; private set; }
		
        public static Item Create(ushort typeId)
        {
            if (ItemsCatalog == null)
                    ItemsCatalog = Game.Instance.ItemLoader.LoadOTItems();

            if (typeId < 100 || !ItemsCatalog.ContainsKey(typeId))
            {
                return null;
                // throw new ArgumentException("Invalid type.", nameof(typeId));
            }

            if (ItemsCatalog[typeId].Flags.Contains(ItemFlag.Container) || ItemsCatalog[typeId].Flags.Contains(ItemFlag.Chest))
            {
                return new Container(ItemsCatalog[typeId]);
            }

            return new Item(ItemsCatalog[typeId]);
        }
    }
}
