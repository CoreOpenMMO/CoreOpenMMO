using System.Collections.Generic;

namespace COMMO.GameServer.OTBParsing {

    public sealed class OldOTBNode {
        public readonly List<OldOTBNode> Children = new List<OldOTBNode>();
        public OTBNodeType Type;
        public int DataBegin;
        public int DataEnd;
    }
}