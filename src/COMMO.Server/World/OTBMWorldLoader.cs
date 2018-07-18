namespace COMMO.Server.World {
	using COMMO.OTB;
	using System;

	/// <summary>
	/// This class contains the methods necessary to load a .otbm file.
	/// </summary>
	public static partial class OTBMWorldLoader {

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
		public static World LoadWorld(ReadOnlyMemory<byte> serializedWorldData) {

			var world = new World();

			var rootNode = OTBDeserializer.DeserializeOTBData(serializedWorldData);
			ParseOTBTreeRootNode(rootNode);

			var worldDataNode = rootNode.Children[0];
			ParseWorldDataNode(worldDataNode, world);

			throw new NotImplementedException();
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
			if (headerVersion == 0 || headerVersion > 2)
				throw new InvalidOperationException();

			var worldWidth = parsingStream.ReadUInt16();
			var worldHeight = parsingStream.ReadUInt16();

			var itemEncodingMajorVersion = parsingStream.ReadUInt32();
			if (itemEncodingMajorVersion != SupportedItemEncodingMajorVersion)
				throw new InvalidOperationException();

			var itemEncodingMinorVersion = parsingStream.ReadUInt32();
			if (itemEncodingMinorVersion < SupportedItemEncodingMinorVersion)
				throw new InvalidOperationException();

			// TODO: use decent loggin methods
			Console.WriteLine($"OTBM header version: {headerVersion}");
			Console.WriteLine($"World width: {worldWidth}");
			Console.WriteLine($"World height: {worldHeight}");
			Console.WriteLine($"Item encoding major version: {itemEncodingMajorVersion}");
			Console.WriteLine($"Item encoding minor version: {itemEncodingMinorVersion}");
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
			var areaZ = (sbyte)stream.ReadByte();

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
		private static void ParseTileNode(
			in Coordinate tilesAreaStartPosition,
			OTBNode tileNode,
			World world
			) {
			if (tileNode == null)
				throw new ArgumentNullException(nameof(tileNode));
			if (world == null)
				throw new ArgumentNullException(nameof(world));
			if (tileNode.Type != OTBNodeType.HouseTile && tileNode.Type != OTBNodeType.NormalTile)
				throw new InvalidOperationException();

			var stream = new OTBParsingStream(tileNode.Data);

			// Finding the tiles "absolute coordinates"
			var xOffset = stream.ReadUInt16();
			var yOffset = stream.ReadUInt16();
			var tilePosition = tilesAreaStartPosition.Translate(
				xOffset: xOffset,
				yOffset: yOffset);

			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Updates the <paramref name="world"/> with the data contained
		/// in <paramref name="tileNode"/>.
		/// </summary>
		private static void ParseTownCollectionNode(OTBNode child, World world) {
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Updates the <paramref name="world"/> with the data contained
		/// in <paramref name="tileNode"/>.
		/// </summary>
		private static void ParseWaypointCollectionNode(OTBNode child, World world) {
			throw new NotImplementedException();
		}
	}
}
