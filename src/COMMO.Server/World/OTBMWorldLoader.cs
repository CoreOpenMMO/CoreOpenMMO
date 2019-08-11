namespace COMMO.Server.World {
	using COMMO.FileFormats.Otb;
	using COMMO.FileFormats.Otbm;
	using COMMO.IO;
	using COMMO.OTB;
	using COMMO.Server.Items;
	using NLog;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Tile = COMMO.Server.Map.Tile;

	/// <summary>
	/// This class contains the methods necessary to load a .otbm file.
	/// </summary>
	public static partial class OTBMWorldLoader {

		/// <summary>
		/// NLog's documentation suggests that we should store a reference to the logger,
		/// instead of asking the LogManager for a new instance everytime we need to log something.
		/// </summary>
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// This class only support items encoded using this major version.
		/// </summary>
		public const uint SupportedItemEncodingMajorVersion = 3;

		/// <summary>
		/// This class only support items encoded using this minor version.
		/// </summary>
		public const uint SupportedItemEncodingMinorVersion = 8;

		/// <summary>
		/// Loads a .otbm file, parse it's contents and returns a <see cref="COMMO.Server.World.World"/>.
		/// </summary>
		public static World LoadWorld(string worldFile) {

			//var world = new World();

			//var otbmFile = OtbmFile.Load(worldFile);

			//foreach (var otbmArea in otbmFile.Areas) {
			//	foreach (var otbmTile in otbmArea.Tiles) {

			//		var pos = otbmArea.Position.Offset(otbmTile.OffsetX, otbmTile.OffsetY, 0);

			//		var tile = new Tile(pos.X, pos.Y, (sbyte) pos.Z);


			//		if (otbmTile.ItemId > 0) {
			//			var ground = ItemFactory2.GetInstance().Create(otbmTile.ItemId);

			//			//ground.

			//			var itemType = new ItemType();

			//			itemType.SetId(ground.Metadata.OpenTibiaId);
			//			itemType.SetClientId(ground.Metadata.TibiaId);
			//			//itemType.SetAttribute(ground.Metadata.TibiaId);
			//			//itemType.SetDescription(ground.de);
			//			//itemType.SetFlag(ground.Metadata.fla);
			//			itemType.SetName(ground.Metadata.Name);

			//			var item = new Items.Item(itemType);

			//			//var item = ItemFactory.Create(otbmTile.ItemId);

			//			tile.AddContent(item);
			//		}

			//		if (otbmTile.Items != null) {
			//			foreach (var otbmItem in otbmTile.Items) {
			//				//var item = ItemFactory.Create(otbmTile.ItemId);

			//				//if (item is Container container) {
			//				//	//TODO: Load container items
			//				//}
			//				//else if (item is StackableItem) {
			//				//	StackableItem stackable = (StackableItem) item;

			//				//	stackable.Count = otbmItem.Count;
			//				//}
			//				//else if (item is TeleportItem) {
			//				//	TeleportItem teleport = (TeleportItem) item;

			//				//	teleport.Position = otbmItem.TeleportPosition;
			//				//}

			//				var ground = ItemFactory2.GetInstance().Create(otbmTile.ItemId);

			//				//ground.

			//				var itemType = new ItemType();

			//				itemType.SetId(ground.Metadata.OpenTibiaId);
			//				itemType.SetClientId(ground.Metadata.TibiaId);
			//				//itemType.SetAttribute(ground.Metadata.TibiaId);
			//				//itemType.SetDescription(ground.de);
			//				//itemType.SetFlag(otbmTile.Flags);
			//				itemType.SetName(ground.Metadata.Name);
			//				//itemType.ParseOTFlags(otbmTile.Flags);

			//				var item = new Items.Item(itemType);

			//				//var item = ItemFactory.Create(otbmTile.ItemId);

			//				tile.AddContent(item);
			//			}
			//		}

			//		world.AddTile(tile);

			//		//if (otbmTile.Items != null) {
			//		//	foreach (var otbmItem in otbmTile.Items) {
			//		//		var item = ItemFactory.Create(otbmTile.ItemId);

			//		//		//if (item is Container container) {
			//		//		//	//TODO: Load container items
			//		//		//}
			//		//		//else if (item is StackableItem) {
			//		//		//	StackableItem stackable = (StackableItem) item;

			//		//		//	stackable.Count = otbmItem.Count;
			//		//		//}
			//		//		//else if (item is TeleportItem) {
			//		//		//	TeleportItem teleport = (TeleportItem) item;

			//		//		//	teleport.Position = otbmItem.TeleportPosition;
			//		//		//}

			//		//		tile.AddContent(item);
			//		//	}
			//		//}
			//	}
			//}

			//var world = new World();

			//using (var stream = new ByteArrayFileTreeStream(worldFile)) {
			//	var reader = new ByteArrayStreamReader(stream);

			//	ParseOTBTreeRootNode(stream, reader, world);

			//	//var worldDataNode = rootNode.Children[0];
			//	//ParseWorldDataNode(worldDataNode, world);

			//	Console.WriteLine($"Tiles Loaded: {world.LoadedTilesCount()}");
			//}

			var ss = File.ReadAllBytes(worldFile);

			var world = new World();

			var rootNode = OTBDeserializer.DeserializeOTBData(
				serializedOTBData: new ReadOnlyMemory<byte>(ss));

			ParseOTBTreeRootNode(rootNode);

			var worldDataNode = rootNode.Children[0];
			ParseWorldDataNode(worldDataNode, world);

			return world;
		}

		/// <summary>
		/// Logs the information embedded in the root node of the OTB tree.
		/// </summary>
		private static void ParseOTBTreeRootNode(ByteArrayFileTreeStream parsingStream, ByteArrayStreamReader reader, World world) {

			parsingStream.Seek(Origin.Current, 6);

			var headerVersion = (OtbmVersion) reader.ReadUInt();

			var worldWidth =reader.ReadUShort();

			var worldHeight = reader.ReadUShort();

			var itemEncodingMajorVersion = (OtbVersion) reader.ReadUInt();

			var itemEncodingMinorVersion = (TibiaVersion) reader.ReadUInt();

			_logger.Info($"OTBM header version: {headerVersion}.");
			_logger.Info($"World width: {worldWidth}.");
			_logger.Info($"World height: {worldHeight}.");
			_logger.Info($"Item encoding major version: {itemEncodingMajorVersion}.");
			_logger.Info($"Item encoding minor version: {itemEncodingMinorVersion}.");

			if (parsingStream.Child()) {

				parsingStream.Seek(Origin.Current, 1);

				var canRead = true;

				while (canRead) {
					switch ((OtbmAttribute) reader.ReadByte()) {
						case OtbmAttribute.Description:
							_logger.Info($"Map Description: {reader.ReadString()}.");

							break;

						case OtbmAttribute.SpawnFile:

							_logger.Info($"SpawnFile: {reader.ReadString()}.");

							break;

						case OtbmAttribute.HouseFile:

							_logger.Info($"HouseFile: {reader.ReadString()}.");

							break;

						default:

							parsingStream.Seek(Origin.Current, -1);
							canRead = false;
							break;
					}
				}

				if (parsingStream.Child()) {
					ParseWorldDataNode(parsingStream, reader, world);
				}

				int a = 10;
			}
		}

		/// <summary>
		/// Updates the <paramref name="world"/> with the data contained
		/// in <paramref name="worldDataNode"/>.
		/// </summary>
		private static void ParseWorldDataNode(ByteArrayFileTreeStream parsingStream, ByteArrayStreamReader reader, World world) {

			var canRead = true;

			while (canRead) {
				switch ((OTBNodeType) reader.ReadByte()) {
					case OTBNodeType.TileArea:
						ParseTileAreaNode(parsingStream, reader, world);
						break;

					case OTBNodeType.TownCollection:
						//ParseTownCollectionNode(parsingStream, reader, world);
						break;

					case OTBNodeType.WayPointCollection:
						//ParseWaypointCollectionNode(parsingStream, reader, world);
						break;
				}

				if (!parsingStream.Next()) {
					canRead = false;
					break;
				}
			}
		}

		/// <summary>
		/// Updates the <paramref name="world"/> with the data contained
		/// in <paramref name="tileAreaNode"/>.
		/// </summary>
		private static void ParseTileAreaNode(ByteArrayFileTreeStream parsingStream, ByteArrayStreamReader reader, World world) {

			var areaStartX = reader.ReadUShort();
			var areaStartY = reader.ReadUShort();
			var areaZ = (sbyte) reader.ReadByte();

			var areaStartPosition = new Coordinate(
				x: areaStartX,
				y: areaStartY,
				z: areaZ);

			if (parsingStream.Child()) {

				while (true) {
					switch ((OtbmType) reader.ReadByte()) {
						case OtbmType.Tile:
							ParseTile(areaStartPosition, parsingStream, reader, world);

							break;

						case OtbmType.HouseTile:
							ParseTile(areaStartPosition, parsingStream, reader, world, true);

							break;
					}

					if (!parsingStream.Next()) {
						break;
					}
				}
			}

			//foreach (var tileNode in tileAreaNode.Children) {
			//	ParseTileNode(
			//		tilesAreaStartPosition: areaStartPosition,
			//		tileNode: tileNode,
			//		world: world);
			//}
		}

		/// <summary>
		/// Updates the <paramref name="world"/> with the data contained
		/// in <paramref name="tileNode"/>.
		/// </summary>
		private static void ParseTile(in Coordinate tilesAreaStartPosition, ByteArrayFileTreeStream parsingStream, ByteArrayStreamReader reader, World world, bool isHouse = false) {
			
			// Finding the tiles "absolute coordinates"
			var xOffset = reader.ReadByte();
			var yOffset = reader.ReadByte();
			var tilePosition = tilesAreaStartPosition.Translate(
				xOffset: xOffset,
				yOffset: yOffset);

			// Currently there's no support for houses
			// Checking whether the tile belongs to a house
			// House house = null;
			if (isHouse) {
				var houseId = reader.ReadUInt();
				//house = HouseManager.Instance.CreateHouseOrGetReference(houseId);
			}

			// We create the tile early and mutate it along the method
			var tile = new Tile((ushort) tilePosition.X, (ushort) tilePosition.Y, tilePosition.Z);

			// Parsing the tile attributes
			var tileFlags = TileFlags.None;
			var tilesItems = new List<Items.Item>();

			var canRead1 = true;

			while (canRead1) {

				var tileNodeAttribute = (OTBMWorldNodeAttribute) reader.ReadByte();
				switch (tileNodeAttribute) {

					case OTBMWorldNodeAttribute.TileFlags:
						var newFlags = (OTBMTileFlags) reader.ReadUInt();
						tileFlags = UpdateTileFlags(tileFlags, newFlags);
						break;

					case OTBMWorldNodeAttribute.Item:
						var itemId = reader.ReadUShort();
						var item = ParseItemData(itemId);
						tile.AddContent(item);
						break;

					default:
						parsingStream.Seek(Origin.Current, -1);

						if (parsingStream.Child()) {

							while (true) {
								var itemId2 = reader.ReadUShort();
								var item2 = ParseItemData(itemId2);
								tile.AddContent(item2);

								if (!parsingStream.Next()) {
									break;
								}
							}
						}

						canRead1 = false;
						break;
				}
			}


			int a = 20;

			//var tileNodeAttribute = (OTBMType) stream.ReadByte();
			//switch (tileNodeAttribute) {
			//	case OTBMType.Tile:

			//		tile = Tile.Load(stream, reader);

			//		break;

			//	case OTBMType.HouseTile:

			//		tile = HouseTile.Load(stream, reader);

			//		break;
			//}



			// var items = tileNode.Children.Select(node => ParseTilesItemNode(node));
//			var items = tileNode
//				.Children
//				.Select(node => new OTBParsingStream(node.Data))
//				.Select(nodeStream => ParseItemData(nodeStream));

//			foreach (var i in items) {
//#warning Not sure if this is the proper method
//				tile.AddContent(i);
//			}
			world.AddTile(tile);
		}

		/// <summary>
		/// Logs the information embedded in the root node of the OTB tree.
		/// </summary>
		private static void ParseOTBTreeRootNode(OTBNode rootNode) {
			if (rootNode == null)
				throw new ArgumentNullException(nameof(rootNode));
			if (rootNode.Children.Count != 1)
				throw new InvalidOperationException();

			var parsingStream = new OTBParsingStream(rootNode.Data);

			var headerVersion = parsingStream.ReadUInt32();
			//if (headerVersion == 0 || headerVersion > 2)
			//	throw new InvalidOperationException();

			var worldWidth = parsingStream.ReadUInt16();
			var worldHeight = parsingStream.ReadUInt16();

			var itemEncodingMajorVersion = parsingStream.ReadUInt32();
			//if (itemEncodingMajorVersion != SupportedItemEncodingMajorVersion)
			//	throw new InvalidOperationException();

			var itemEncodingMinorVersion = parsingStream.ReadUInt32();
			//if (itemEncodingMinorVersion < SupportedItemEncodingMinorVersion)
			//	throw new InvalidOperationException();

			_logger.Info($"OTBM header version: {headerVersion}.");
			_logger.Info($"World width: {worldWidth}.");
			_logger.Info($"World height: {worldHeight}.");
			_logger.Info($"Item encoding major version: {itemEncodingMajorVersion}.");
			_logger.Info($"Item encoding minor version: {itemEncodingMinorVersion}.");
		}

		/// <summary>
		/// Updates the <paramref name="world"/> with the data contained
		/// in <paramref name="worldDataNode"/>.
		/// </summary>
		private static void ParseWorldDataNode(OTBNode worldDataNode, World world) {
			if (worldDataNode == null)
				throw new ArgumentNullException(nameof(worldDataNode));
			if (world == null)
				throw new ArgumentNullException(nameof(world));
			if (worldDataNode.Type != OTBNodeType.WorldData)
				throw new InvalidOperationException();

			foreach (var child in worldDataNode.Children) {
				switch (child.Type) {
					case OTBNodeType.TileArea:
						ParseTileAreaNode(child, world);
						break;

					case OTBNodeType.TownCollection:
						ParseTownCollectionNode(child, world);
						break;

					case OTBNodeType.WayPointCollection:
						ParseWaypointCollectionNode(child, world);
						break;

					case OTBNodeType.ItemDefinition:
						throw new NotImplementedException("TFS didn't implement this. So didn't we.");

					default:
						throw new InvalidOperationException();
				}
			}
		}

		/// <summary>
		/// Updates the <paramref name="world"/> with the data contained
		/// in <paramref name="tileAreaNode"/>.
		/// </summary>
		private static void ParseTileAreaNode(OTBNode tileAreaNode, World world) {
			if (tileAreaNode == null)
				throw new ArgumentNullException(nameof(tileAreaNode));
			if (tileAreaNode.Type != OTBNodeType.TileArea)
				throw new InvalidOperationException();

			var stream = new OTBParsingStream(tileAreaNode.Data);

			var areaStartX = stream.ReadUInt16();
			var areaStartY = stream.ReadUInt16();
			var areaZ = (sbyte) stream.ReadByte();

			var areaStartPosition = new Coordinate(
				x: areaStartX,
				y: areaStartY,
				z: areaZ);

			foreach (var tileNode in tileAreaNode.Children) {
				ParseTileNode(
					tilesAreaStartPosition: areaStartPosition,
					tileNode: tileNode,
					world: world);
			}
		}

		/// <summary>
		/// Updates the <paramref name="world"/> with the data contained
		/// in <paramref name="tileNode"/>.
		/// </summary>
		private static void ParseTileNode(in Coordinate tilesAreaStartPosition, OTBNode tileNode, World world) {

			try {
				if (tileNode == null)
					throw new ArgumentNullException(nameof(tileNode));
				if (world == null)
					throw new ArgumentNullException(nameof(world));
				if (tileNode.Type != OTBNodeType.HouseTile && tileNode.Type != OTBNodeType.NormalTile)
					throw new InvalidOperationException();

				var stream = new OTBParsingStream(tileNode.Data);

				// Finding the tiles "absolute coordinates"
				var xOffset = stream.ReadByte();
				var yOffset = stream.ReadByte();
				var tilePosition = tilesAreaStartPosition.Translate(
					xOffset: xOffset,
					yOffset: yOffset);

				// Currently there's no support for houses
				// Checking whether the tile belongs to a house
				// House house = null;
				if (tileNode.Type == OTBNodeType.HouseTile) {
					var houseId = stream.ReadUInt32();
					//house = HouseManager.Instance.CreateHouseOrGetReference(houseId);
				}

				// We create the tile early and mutate it along the method
				var tile = new Tile((ushort) tilePosition.X, (ushort) tilePosition.Y, tilePosition.Z);

				// Parsing the tile attributes
				var tileFlags = TileFlags.None;
				var tilesItems = new List<Items.Item>();

				var tileNodeAttribute = (OTBMWorldNodeAttribute) stream.ReadByte();
				switch (tileNodeAttribute) {

					case OTBMWorldNodeAttribute.TileFlags:
						var newFlags = (OTBMTileFlags) stream.ReadUInt32();
						tileFlags = UpdateTileFlags(tileFlags, newFlags);
						break;

					case OTBMWorldNodeAttribute.Item:
						var item = ParseItemData(stream);
#warning Not sure if this is the proper method
						tile.AddContent(item);
						break;

					default:
						var item2 = ParseItemData(stream);

						_logger.Info($"Default Id:::: {item2.ThingId}");

#warning Not sure if this is the proper method
						//tile.AddContent(item2);
						break;
						//throw new Exception("TFS just threw a exception here, so shall we... Reason: unknown tile attribute.");
				}

				// var items = tileNode.Children.Select(node => ParseTilesItemNode(node));
				var items = tileNode
					.Children
					.Select(node => new OTBParsingStream(node.Data))
					.Select(nodeStream => ParseItemData(nodeStream));

				foreach (var i in items) {
#warning Not sure if this is the proper method
					tile.AddContent(i);
				}
				world.AddTile(tile);
			}
			catch (Exception ex) { _logger.Info($"EX: {ex.Message}"); }
		}


		private static TileFlags UpdateTileFlags(TileFlags oldFlags, OTBMTileFlags newFlags) {
			if ((newFlags & OTBMTileFlags.NoLogout) != 0)
				oldFlags |= TileFlags.NoLogout;

			// I think we should throw if a tile contains contradictory flags, instead of just
			// ignoring them like tfs does...
			if ((newFlags & OTBMTileFlags.ProtectionZone) != 0)
				oldFlags |= TileFlags.ProtectionZone;
			else if ((newFlags & OTBMTileFlags.NoPvpZone) != 0)
				oldFlags |= TileFlags.NoPvpZone;
			else if ((newFlags & OTBMTileFlags.PvpZone) != 0)
				oldFlags |= TileFlags.PvpZone;

			return oldFlags;
		}

		/// <summary>
		/// Updates the <paramref name="world"/> with the data contained
		/// in <paramref name="tileNode"/>.
		/// </summary>
		private static void ParseTownCollectionNode(OTBNode townCollectionNode, World world) {
			if (townCollectionNode == null)
				throw new ArgumentNullException(nameof(townCollectionNode));
			if (world == null)
				throw new ArgumentNullException(nameof(world));

			foreach (var townNode in townCollectionNode.Children) {
				if (townNode.Type != OTBNodeType.Town)
					throw new InvalidOperationException();

				var stream = new OTBParsingStream(townNode.Data);

				var townId = stream.ReadUInt32();
				// Implement Town and TownManager

				var townName = stream.ReadString();
				// Set town name

				var townTempleX = stream.ReadUInt16();
				var townTempleY = stream.ReadUInt16();
				var townTempleZ = stream.ReadByte();
				// Set town's temple
			}
		}

		/// <summary>
		/// Updates the <paramref name="world"/> with the data contained
		/// in <paramref name="tileNode"/>.
		/// </summary>
		private static void ParseWaypointCollectionNode(OTBNode waypointCollection, World world) {
			if (waypointCollection == null)
				throw new ArgumentNullException(nameof(waypointCollection));
			if (world == null)
				throw new ArgumentNullException(nameof(world));

			foreach (var waypointNode in waypointCollection.Children) {
				if (waypointNode.Type != OTBNodeType.WayPoint)
					throw new InvalidOperationException();

				var stream = new OTBParsingStream(waypointNode.Data);

				var waypointName = stream.ReadString();

				var waypointX = stream.ReadUInt16();
				var waypointY = stream.ReadUInt16();
				var waypointZ = stream.ReadBool();

				// Implement waypoints
			}
		}
	}
}
