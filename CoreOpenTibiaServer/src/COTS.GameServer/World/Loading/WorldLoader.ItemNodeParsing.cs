using System.Collections.Generic;

namespace COTS.GameServer.World.Loading {

    public static partial class WorldLoader {

        public static void ParseItemNode(
            ParsingTree parsingTree,
            ParsingNode parsingNode
            ) {
            if (parsingNode.Type != NodeType.Item)
                throw new MalformedItemNodeException();

            var stream = new WorldParsingStream(parsingTree, parsingNode);

            var itemId = stream.ReadUInt16();
            var item = Item.CreateFromId(itemId);
        }

        private static List<ItemAttribute> DeserializeAttribute(ref WorldParsingStream stream) {
#warning Implement this
            return new List<ItemAttribute>();
        }
    }
}