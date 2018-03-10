using COMMO.GameServer.Items;
using COMMO.GameServer.OTBParsing;

namespace COMMO.GameServer.World.Loading {

	public static partial class TFSWorldLoader {

		private static void ParseItemNode(
			OldOTBTree parsingTree,
			OldOTBNode parsingNode
			) {
			if (parsingNode.Type != OTBNodeType.Item)
				throw new MalformedItemNodeException();

#warning Implement this
		}
	}
}