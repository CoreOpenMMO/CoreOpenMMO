// <copyright file="ItemFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using COMMO.Common.Objects;
using COMMO.Common.Structures;
using COMMO.Data.Contracts;
using COMMO.FileFormats.Dat;
using COMMO.FileFormats.Otb;
using COMMO.FileFormats.Xml.Items;

namespace COMMO.Server.Items {
	public static class ItemFactory {
		public static object InitLock = new object();

		public static Dictionary<ushort, ItemType> ItemsCatalog { get; private set; }

		public static Item Create(ushort typeId) {
			if (ItemsCatalog == null)
				ItemsCatalog = Game.Instance.ItemLoader.LoadOTItems();

			if (typeId < 100 || !ItemsCatalog.ContainsKey(typeId)) {
				return null;
				// throw new ArgumentException("Invalid type.", nameof(typeId));
			}

			if (ItemsCatalog[typeId].Flags.Contains(ItemFlag.Container) || ItemsCatalog[typeId].Flags.Contains(ItemFlag.Chest)) {
				return new Container(ItemsCatalog[typeId]);
			}

			return new Item(ItemsCatalog[typeId]);
		}
	}

	public class ItemFactory2 {

		private static ItemFactory2 _instance;

		public static ItemFactory2 GetInstance() 
		{
			if (_instance == null)
				_instance = new ItemFactory2();

			return _instance;
		}

		private ItemFactory2() {

			var otbFile = OtbFile.Load("data/items/items.otb");
			var datFile = DatFile.Load("data/items/tibia.dat");
			var itemsFile = ItemsFile.Load("data/items/items.xml");

			_metadatas = new Dictionary<ushort, ItemMetadata>(datFile.Items.Count);

			foreach (var otbItem in otbFile.Items) {
				if (otbItem.Group != FileFormats.Otb.ItemGroup.Deprecated) {
					_metadatas.Add(otbItem.OpenTibiaId, new ItemMetadata() {
						TibiaId = otbItem.TibiaId,

						OpenTibiaId = otbItem.OpenTibiaId
					});
				}
			}

			var lookup = otbFile.Items.ToLookup(otbItem => otbItem.TibiaId, otbItem => otbItem.OpenTibiaId);

			foreach (var datItem in datFile.Items) {
				foreach (var openTibiaId in lookup[datItem.TibiaId]) {
					var metadata = _metadatas[openTibiaId];

					metadata.TopOrder = datItem.IsGround ? TopOrder.Ground

														 : datItem.AlwaysOnTop1 ? TopOrder.HighPriority

																				: datItem.AlwaysOnTop2 ? TopOrder.MediumPriority

																									   : datItem.AlwaysOnTop3 ? TopOrder.LowPriority

																															  : TopOrder.Other;

					metadata.Speed = datItem.Speed;

					metadata.IsContainer = datItem.IsContainer;

					metadata.Stackable = datItem.Stackable;

					metadata.NotWalkable = datItem.NotWalkable;

					metadata.BlockProjectile = datItem.BlockProjectile;

					metadata.BlockPathFinding = datItem.BlockPathFinding;

					//TODO: Set other properties
				}
			}

			foreach (var xmlItem in itemsFile.Items) {
				if (xmlItem.OpenTibiaId < 20000) {
					var metadata = _metadatas[xmlItem.OpenTibiaId];

					metadata.Name = xmlItem.Name;

					metadata.Capacity = xmlItem.ContainerSize;

					//TODO: Set other properties
				}
			}
		}

		private readonly Dictionary<ushort, ItemMetadata> _metadatas;

		public Common.Objects.Item Create(ushort openTibiaId) {

			if (!_metadatas.TryGetValue(openTibiaId, out var metadata)) {
				return null;
			}

			if (metadata.IsContainer) {
				return new Common.Objects.Container(metadata);
			}
			else if (metadata.Stackable) {
				return new StackableItem(metadata);
			}

			return new Common.Objects.Item(metadata);
		}
	}
}
