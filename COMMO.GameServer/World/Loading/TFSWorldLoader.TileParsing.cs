using COMMO.GameServer.OTBParsing;
using System;

namespace COMMO.GameServer.World.Loading {

	public static partial class TFSWorldLoader {

		private static void ParseTileAreaNode(OTBNode tileAreaNode, World world) {
			if (tileAreaNode == null)
				throw new ArgumentNullException(nameof(tileAreaNode));
			if (tileAreaNode.Type != OTBNodeType.TileArea)
				throw new WorldLoadingException();

			var stream = new OTBParsingStream(tileAreaNode.Data.Span);

			var areaStartX = stream.ReadUInt16();
			var areaStartY = stream.ReadUInt16();
			var areaZ = stream.ReadByte();

			var areaStartPosition = new Position(
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

		private static void ParseTileNode(
			in Position tilesAreaStartPosition,
			OTBNode tileNode,
			World world
			) {

			throw new NotImplementedException();
		}
	}
}