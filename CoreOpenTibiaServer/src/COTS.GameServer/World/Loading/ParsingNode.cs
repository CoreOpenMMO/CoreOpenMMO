using System.Collections.Generic;

namespace COTS.GameServer.World.Loading {

    public sealed class ParsingNode {
        public readonly List<ParsingNode> Children = new List<ParsingNode>();
        public NodeType Type;
        public int PropsBegin;
        public int PropsEnd;
    }
}