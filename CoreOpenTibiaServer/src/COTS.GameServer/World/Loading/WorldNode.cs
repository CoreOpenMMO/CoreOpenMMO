using System.Collections.Generic;

namespace COTS.GameServer.World.Loading {

    public sealed class WorldNode {

        public readonly List<WorldNode> Children = new List<WorldNode>();
        public byte Type;
        public int PropsBegin;
        public int PropsEnd;
    }
}