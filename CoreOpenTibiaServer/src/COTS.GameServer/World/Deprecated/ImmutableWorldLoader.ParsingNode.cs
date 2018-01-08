using System.Collections.Generic;

namespace COTS.GameServer.World {

    public static partial class ImmutableWorldLoader {

        private sealed class ParsingWorldNode {

            public enum NodeMarker : byte {
                Escape = 0xFD,
                Start = 0XFE,
                End = 0xFF
            }

            public readonly List<ParsingWorldNode> Children = new List<ParsingWorldNode>();
            public byte Type;

            public int PropsBegin;
            public int PropsEnd;
        }
    }
}