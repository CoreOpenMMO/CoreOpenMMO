using System;

namespace COTS.GameServer.World {

    public sealed class WorldNode {
        public readonly ArraySegment<byte> Props;
        public readonly ReadOnlyArray<WorldNode> Children;

        public WorldNode(
            ArraySegment<byte> props,
            ReadOnlyArray<WorldNode> children
            ) {
            if (props == null)
                throw new ArgumentNullException(nameof(props));
            if (children == null)
                throw new ArgumentNullException(nameof(children));

            Props = props;
            Children = children;
        }
    }
}