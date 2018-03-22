using COMMO.GameServer.OTBParsing;
using System;

namespace COMMO.GameServer.World.TFSLoading {

	public static partial class TFSWorldLoader {

		/*
			OTBM_ROOTV1
			|
			|--- OTBM_MAP_DATA
			|	|
			|	|--- OTBM_TILE_AREA
			|	|	|--- OTBM_TILE
			|	|	|--- OTBM_TILE_SQUARE (not implemented)
			|	|	|--- OTBM_TILE_REF (not implemented)
			|	|	|--- OTBM_HOUSETILE
			|	|
			|	|--- OTBM_SPAWNS (not implemented)
			|	|	|--- OTBM_SPAWN_AREA (not implemented)
			|	|	|--- OTBM_MONSTER (not implemented)
			|	|
			|	|--- OTBM_TOWNS
			|	|	|--- OTBM_TOWN
			|	|
			|	|--- OTBM_WAYPOINTS
			|		|--- OTBM_WAYPOINT
			|
			|--- OTBM_ITEM_DEF (not implemented)
		*/

		public const uint SupportedItemEncodingMajorVersion = 3;
		public const uint SupportedItemEncodingMinorVersion = 8;

		public static World ParseWorld(byte[] serializedWorldData) {
			if (serializedWorldData.Length < MinimumWorldSize)
				throw new TFSWorldLoadingException();

			var otbTree = ExtractOTBTree(serializedWorldData);
			var world = new World();

			ParseOTBTreeRootNode(otbTree, world);
			Parse(otbTree.Children[0], world);

			throw new NotImplementedException();
		}

		private static void ParseOTBTreeRootNode(OTBNode rootNode, World world) {
			if (rootNode == null)
				throw new ArgumentNullException(nameof(rootNode));
			if (world == null)
				throw new ArgumentNullException(nameof(world));
			if (rootNode.Children.Count != 1)
				throw new TFSWorldLoadingException();

			var parsingStream = new OTBParsingStream(rootNode.Data.Span);

			var headerVersion = parsingStream.ReadUInt32();
			if (headerVersion == 0 || headerVersion > 2)
				throw new TFSWorldLoadingException();

			var worldWidth = parsingStream.ReadUInt16();
			var worldHeight = parsingStream.ReadUInt16();

			var itemEncodingMajorVersion = parsingStream.ReadUInt32();
			if (itemEncodingMajorVersion != SupportedItemEncodingMajorVersion)
				throw new TFSWorldLoadingException();

			var itemEncodingMinorVersion = parsingStream.ReadUInt32();
			if (itemEncodingMinorVersion < SupportedItemEncodingMinorVersion)
				throw new TFSWorldLoadingException();

			Console.WriteLine($"OTBM header version: {headerVersion}");
			Console.WriteLine($"World width: {parsingStream.ReadUInt16()}");
			Console.WriteLine($"World height: {parsingStream.ReadUInt16()}");
			Console.WriteLine($"Item encoding major version: {parsingStream.ReadUInt32()}");
			Console.WriteLine($"Item encoding minor version: {parsingStream.ReadUInt32()}");
		}

		private static void Parse(OTBNode worldDataNode, World world) {
			if (worldDataNode == null)
				throw new ArgumentNullException(nameof(worldDataNode));
			if (world == null)
				throw new ArgumentNullException(nameof(world));
			if (worldDataNode.Type != OTBNodeType.WorldData)
				throw new TFSWorldLoadingException();

			foreach (var child in worldDataNode.Children) {
				switch (child.Type) {
					case OTBNodeType.TileArea:
					ParseTileAreaNode(child, world);
					break;

					case OTBNodeType.TownCollection:
					// ParseTownCollectionNode(child, world);
					break;

					case OTBNodeType.WayPointCollection:
					// ParseWaypointCollectionNode(child, world);
					break;

					case OTBNodeType.ItemDefinition:
					throw new NotImplementedException("TFS didn't implement this. So didn't we.");

					default:
					throw new TFSWorldLoadingException();
				}
			}
		}
	}
}
