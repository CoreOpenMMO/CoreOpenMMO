using System.Collections.Generic;

namespace COMMO.GameServer.OTBParsing {

    public sealed class OTBNode {
        public readonly List<OTBNode> Children = new List<OTBNode>();
        public OTBNodeType Type;
        public int DataBegin;
        public int DataEnd;
    }
}