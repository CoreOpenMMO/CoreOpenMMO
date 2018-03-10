using COMMO.GameServer.OTBParsing;
using System;

namespace COMMO.GameServer.World.Loading {

	public static partial class TFSWorldLoader {

		private static void ParseTileAreaNode(OldOTBTree parsingTree, OldOTBNode tileAreaNode) {
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
			OldOTBTree parsingTree,
			OldOTBNode tileNode,
			UInt16 areaStartingX,
			UInt16 areaStartingY,
			Byte areaZ
			) {
			if (tileNode.Type != OTBNodeType.NormalTile && tileNode.Type != OTBNodeType.HouseTile)
				throw new MalformedTileAreaNodeException("Unknow tile area node type.");

			var stream = new OTBNodeParsingStream(parsingTree, tileNode);

			var tileXOffset = stream.ReadByte();
			var tileYOffset = stream.ReadByte();
			var tilePosition = new Position(
				x: areaStartingX + tileXOffset,
				y: areaStartingY + tileYOffset,
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
		}
	}
}