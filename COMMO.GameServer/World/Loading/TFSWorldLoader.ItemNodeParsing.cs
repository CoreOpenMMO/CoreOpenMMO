using COMMO.GameServer.Items;
using COMMO.GameServer.OTBParsing;

namespace COMMO.GameServer.World.Loading {

	public static partial class TFSWorldLoader {

		private static Item ParseItemNode(
			OTBTree parsingTree,
			OTBNode parsingNode
			) {
			if (parsingNode.Type != OTBNodeType.Item)
				throw new MalformedItemNodeException();

#warning Implement this
			return null;
		}
	}
}