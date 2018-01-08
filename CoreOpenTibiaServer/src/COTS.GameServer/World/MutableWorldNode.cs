using System.Collections.Generic;

namespace COTS.GameServer.World {

    public sealed class MutableWorldNode {

        public enum NodeMarker : byte {
            Escape = 0xFD,
            Start = 0XFE,
            End = 0xFF
        }

        public readonly List<MutableWorldNode> Children = new List<MutableWorldNode>();
        public byte Type;
        public int PropsBegin;
        public int PropsEnd;
    }
}