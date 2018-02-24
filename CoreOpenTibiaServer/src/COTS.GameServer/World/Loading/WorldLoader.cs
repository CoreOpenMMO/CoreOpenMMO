using COTS.GameServer.OTBParsing;
using System;

namespace COTS.GameServer.World.Loading {

	public static partial class WorldLoader {

		public static ParsingTree ParseWorld(byte[] serializedWorldData) {
			if (serializedWorldData == null)
				throw new ArgumentNullException(nameof(serializedWorldData));
			if (serializedWorldData.Length < MinimumWorldSize)
				throw new MalformedWorldException();

			var stream = new ByteArrayReadStream(serializedWorldData);
			var rootNode = ParseTree(stream);
			return new ParsingTree(serializedWorldData, rootNode);
		}

		public static void LoadWorld(ref ParsingTree mapTree) {
			var rootNode = mapTree.Root;

			var mapNode = rootNode.Children[0];
			if (mapNode == null)
				throw new MalformedWorldException();

			foreach (var mapDataNode in mapNode.Children) {
				switch (mapDataNode.Type) {
					case NodeType.TileArea:
					WorldLoader.ParseTileAreaNode(mapTree, mapDataNode);
					break;

					//default:
					//throw new NotImplementedException();
				}
			}
		}
	}
}