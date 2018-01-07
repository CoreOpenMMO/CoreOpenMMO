using System.Collections.Generic;

namespace COTS.GameServer.World {

    public sealed class WorldNode {

        public enum NodeMarker : byte {
            Escape = 0xFD,
            Start = 0XFE,
            End = 0xFF
        }

        public readonly List<WorldNode> Children = new List<WorldNode>();
        public byte Type;

        public int PropsBegin;
        public int PropsEnd;
    }
}