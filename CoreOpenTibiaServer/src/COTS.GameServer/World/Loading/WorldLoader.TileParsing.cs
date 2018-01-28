using COTS.GameServer.OTBParsing;
using System;

namespace COTS.GameServer.World.Loading {

	public static partial class WorldLoader {

		public static void ParseTileAreaNode(ParsingTree parsingTree, ParsingNode tileAreaNode) {
			if (parsingTree == null)
				throw new ArgumentNullException(nameof(parsingTree));
			if (tileAreaNode == null)
				throw new ArgumentNullException(nameof(tileAreaNode));

			var stream = new ParsingStream(
				tree: parsingTree,
				node: tileAreaNode);

			var areaStartingX = stream.ReadUInt16();
			var areaStartingY = stream.ReadUInt16();
			var areaZ = stream.ReadByte();

			foreach (var tileNode in tileAreaNode.Children) {
				ParseTileNode(
					parsingTree: parsingTree,
					tileNode: tileNode,
					areaStartingX: areaStartingX,
					areaStartingY: areaStartingY,
					areaZ: areaZ);
			}
		}

		private static void ParseTileNode(
			ParsingTree parsingTree,
			ParsingNode tileNode,
			UInt16 areaStartingX,
			UInt16 areaStartingY,
			Byte areaZ
			) {
			if (tileNode.Type != NodeType.NormalTile && tileNode.Type != NodeType.HouseTile)
				throw new MalformedTileAreaNodeException("Unknow tile area node type.");

			var stream = new ParsingStream(parsingTree, tileNode);

			var tileX = areaStartingX + stream.ReadByte();
			var tileY = areaStartingY + stream.ReadByte();

			Tile tile = null;
			if (tileNode.Type == NodeType.HouseTile)
				tile = ParseHouseTile(ref stream, (ushort)tileX, (ushort)tileY, areaZ); // Improve this, remove casts

			var tileFlags = ParseTileAttributes(ref parsingTree, ref stream, ref tile, tileNode);
			if (tile != null)
				tile.Flags.AddFlags(tileFlags);
		}

		private static Tile ParseHouseTile(ref ParsingStream stream, UInt16 tileX, UInt16 tileY, byte tileZ) {
			var houseId = stream.ReadUInt32();
			var house = HouseManager.Instance.CreateHouseOrGetReference(houseId);
			var tile = HouseTile.CreateTileAndAddItToHouse(tileX, tileY, tileZ, house);

			return tile;
		}

		private static TileFlags ParseTileAttributes(ref ParsingTree parsingTree, ref ParsingStream stream, ref Tile tile, ParsingNode tileNode) {
			var tileFlags = new TileFlags();
			NodeAttribute nodeAttribute;
			while (!stream.IsOver) {
				nodeAttribute = (NodeAttribute)stream.ReadByte();
				switch (nodeAttribute) {
					case NodeAttribute.TileFlags:
					tileFlags = ParseTileFlags(ref stream);
					break;

					case NodeAttribute.Item:
					ParseTileItem(ref stream, ref tile, tileNode.Type == NodeType.HouseTile);
					break;

					default:
					throw new MalformedTileNodeException("Unknow node attribute " + nameof(nodeAttribute) + " of type " + nodeAttribute);
				}
			}

			foreach (var itemNode in tileNode.Children) {
				if (itemNode.Type != NodeType.Item)
					throw new MalformedItemNodeException();

				var itemStream = new ParsingStream(parsingTree, itemNode);
				ParseTileItem(ref itemStream, ref tile, tileNode.Type == NodeType.HouseTile);
			}

			return tileFlags;
		}

		private static TileFlags ParseTileFlags(ref ParsingStream stream) {
			var flags = (TileFlags)stream.ReadUInt32();
			var tileFlags = TileFlags.None;

			if (TileFlagsExtensions.FlagsAreSet(flags, (TileFlags)OTBMTileFlag.NoLogout))
				TileFlagsExtensions.AddFlags(tileFlags, (TileFlags)OTBMTileFlag.NoLogout);

			if (TileFlagsExtensions.FlagsAreSet(flags, (TileFlags)OTBMTileFlag.NoPvpZone))
				TileFlagsExtensions.AddFlags(tileFlags, (TileFlags)OTBMTileFlag.NoPvpZone);
			else if (TileFlagsExtensions.FlagsAreSet(flags, (TileFlags)OTBMTileFlag.PvpZone))
				TileFlagsExtensions.AddFlags(tileFlags, (TileFlags)OTBMTileFlag.PvpZone);
			else if (TileFlagsExtensions.FlagsAreSet(flags, (TileFlags)OTBMTileFlag.ProtectionZone))
				TileFlagsExtensions.AddFlags(tileFlags, (TileFlags)OTBMTileFlag.ProtectionZone);

			return tileFlags;
		}

		private static void ParseTileItem(ref ParsingStream stream, ref Tile tile, bool isHouse = false) {
			var itemId = stream.ReadUInt16();
			var item = new Items.Item(itemId);

			if (isHouse && item.IsMoveable())
				throw new MoveableItemInHouseException(); // Don't need to use Exception. Maybe just a warning with pos and don't place the item in tile is Okay
			else {
				// if item.count <= 0; item.count = 1. But if always create with count = 0 this should only be item.count = 1, ou create with count = 1
				if (tile != null) {
					//tile.AddInternalThing(null); // Item
					// item.StartDecaying();
					// item.LoadedFromMap = true;
				} else if (item != null) { // item is ground Tile
										   // ground_item = item
				} else {
					tile = new Tile(0, 0, 0); // XYZ
											  //tile.AddInternalThing(null); // Item
											  // item.StartDecaying();
											  // item.LoadedFromMap = true;
				}
			}
		}
	}
}