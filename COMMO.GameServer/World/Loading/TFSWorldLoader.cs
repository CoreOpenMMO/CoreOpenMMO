using COMMO.GameServer.OTBParsing;
using System;

namespace COMMO.GameServer.World.Loading {

	public static partial class TFSWorldLoader {

		public static World ParseWorld(byte[] serializedWorldData) {
			if (serializedWorldData == null)
				throw new ArgumentNullException(nameof(serializedWorldData));
			if (serializedWorldData.Length < MinimumWorldSize)
				throw new MalformedWorldException();

			var worldParsingTree = ExtractOTBTree(serializedWorldData);
			var mapNode = worldParsingTree.Root.Children[0];
			if (mapNode == null)
				throw new MalformedWorldException();

			foreach (var mapDataNode in mapNode.Children) {
				switch (mapDataNode.Type) {
					case OTBNodeType.TileArea:
					TFSWorldLoader.ParseTileAreaNode(worldParsingTree, mapDataNode);
					break;

					//default:
					//throw new NotImplementedException();
				}
			}

			throw new NotImplementedException();

		}
	}
}