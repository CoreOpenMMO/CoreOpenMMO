using COMMO.GameServer.OTBParsing;
using System;

namespace COMMO.GameServer.World.Loading {

	public static partial class TFSWorldLoader {

		private static void ParseTileAreaNode(OTBTree parsingTree, OldOTBNode tileAreaNode) {
			if (parsingTree == null)
				throw new ArgumentNullException(nameof(parsingTree));
			if (tileAreaNode == null)
				throw new ArgumentNullException(nameof(tileAreaNode));

			var stream = new OTBNodeParsingStream(
				tree: parsingTree,
				nodeToParse: tileAreaNode);

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
			OTBTree parsingTree,
			OldOTBNode tileNode,
			UInt16 areaStartingX,
			UInt16 areaStartingY,
			Byte areaZ
			) {
			if (tileNode.Type != OTBNodeType.NormalTile && tileNode.Type != OTBNodeType.HouseTile)
				throw new MalformedTileAreaNodeException("Unknow tile area node type.");

			var stream = new OTBNodeParsingStream(parsingTree, tileNode);

			var tileX = areaStartingX + stream.ReadByte();
			var tileY = areaStartingY + stream.ReadByte();
			var tilePosition = new Position(
				x: tileX,
				y: tileY,
				z: areaZ);

			Tile tile = null;
			if (tileNode.Type == OTBNodeType.HouseTile) {
				tile = new Tile(
					position: tilePosition,
					belongsToHouse: true);

				var houseId = stream.ReadUInt32();
				var house = HouseManager.Instance.CreateHouseOrGetReference(houseId);
				house.AddTile(tile);
			} else {
				tile = new Tile(
					position: tilePosition,
					belongsToHouse: false);
			}

			var tileFlags = ParseTileAttributes(ref parsingTree, ref stream, ref tile, tileNode);
			if (tile != null)
				tile.Flags.AddFlags(tileFlags);
		}

		private static TileFlags ParseTileAttributes(ref OTBTree parsingTree, ref OTBNodeParsingStream stream, ref Tile tile, OldOTBNode tileNode) {
			var tileFlags = new TileFlags();
			TFSWorldNodeAttribute nodeAttribute;
			while (!stream.IsOver) {
				nodeAttribute = (TFSWorldNodeAttribute)stream.ReadByte();
				switch (nodeAttribute) {
					case TFSWorldNodeAttribute.TileFlags:
					tileFlags = ParseTileFlags(ref stream);
					break;

					case TFSWorldNodeAttribute.Item:
					ParseTileItem(ref stream, ref tile, tileNode.Type == OTBNodeType.HouseTile);
					break;

					default:
					throw new MalformedTileNodeException("Unknow node attribute " + nameof(nodeAttribute) + " of type " + nodeAttribute);
				}
			}

			foreach (var itemNode in tileNode.Children) {
				if (itemNode.Type != OTBNodeType.Item)
					throw new MalformedItemNodeException();

				var itemStream = new OTBNodeParsingStream(parsingTree, itemNode);
				ParseTileItem(ref itemStream, ref tile, tileNode.Type == OTBNodeType.HouseTile);
			}

			return tileFlags;
		}

		private static TileFlags ParseTileFlags(ref OTBNodeParsingStream stream) {
			var flags = (TileFlags)stream.ReadUInt32();
			var tileFlags = TileFlags.None;

			if (TileFlagsExtensions.FlagsAreSet(flags, (TileFlags)TFSTileFlag.NoLogout))
				TileFlagsExtensions.AddFlags(tileFlags, (TileFlags)TFSTileFlag.NoLogout);

			if (TileFlagsExtensions.FlagsAreSet(flags, (TileFlags)TFSTileFlag.NoPvpZone))
				TileFlagsExtensions.AddFlags(tileFlags, (TileFlags)TFSTileFlag.NoPvpZone);
			else if (TileFlagsExtensions.FlagsAreSet(flags, (TileFlags)TFSTileFlag.PvpZone))
				TileFlagsExtensions.AddFlags(tileFlags, (TileFlags)TFSTileFlag.PvpZone);
			else if (TileFlagsExtensions.FlagsAreSet(flags, (TileFlags)TFSTileFlag.ProtectionZone))
				TileFlagsExtensions.AddFlags(tileFlags, (TileFlags)TFSTileFlag.ProtectionZone);

			return tileFlags;
		}

		private static void ParseTileItem(ref OTBNodeParsingStream stream, ref Tile tile, bool isHouse = false) {
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