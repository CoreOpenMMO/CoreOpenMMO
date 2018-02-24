using COMMO.GameServer.OTBParsing;
using System.Collections.Generic;
using COMMO.GameServer.Items;

namespace COMMO.GameServer.World.Loading {

    public static partial class WorldLoader {

        public static void ParseItemNode(
            ParsingTree parsingTree,
            ParsingNode parsingNode
            ) {
            if (parsingNode.Type != NodeType.Item)
                throw new MalformedItemNodeException();

            var stream = new ParsingStream(parsingTree, parsingNode);

            var itemId = stream.ReadUInt16();
            // var item = Item.CreateFromId(itemId);
        }

        private static List<ItemAttribute> DeserializeAttribute(ref ParsingStream stream) {
            return new List<ItemAttribute>();
        }
    }
}