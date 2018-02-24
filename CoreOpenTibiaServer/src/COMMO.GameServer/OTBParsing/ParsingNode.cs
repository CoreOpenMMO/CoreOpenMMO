using System.Collections.Generic;

namespace COMMO.GameServer.OTBParsing {

    public sealed class ParsingNode {
        public readonly List<ParsingNode> Children = new List<ParsingNode>();
        public NodeType Type;
        public int DataBegin;
        public int DataEnd;
    }
}