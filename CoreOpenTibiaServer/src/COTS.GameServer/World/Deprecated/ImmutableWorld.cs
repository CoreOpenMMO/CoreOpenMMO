using System;

namespace COTS.GameServer.World {

    public sealed class ImmutableWorld {
        public const ushort MapMaximumLayers = 15;
        public readonly ImmutableWorldNode Root;

        public ImmutableWorld(ImmutableWorldNode root) {
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            Root = root;
        }
    }
}