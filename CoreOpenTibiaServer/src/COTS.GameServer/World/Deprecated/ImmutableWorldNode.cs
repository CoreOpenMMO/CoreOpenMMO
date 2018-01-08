using System;

namespace COTS.GameServer.World {

    public sealed class ImmutableWorldNode {
        public readonly ArraySegment<byte> Props;
        public readonly ReadOnlyArray<ImmutableWorldNode> Children;

        public ImmutableWorldNode(
            ArraySegment<byte> props,
            ReadOnlyArray<ImmutableWorldNode> children
            ) {
            if (children == null)
                throw new ArgumentNullException(nameof(children));

            Props = props;
            Children = children;
        }
    }
}