using System;

namespace COTS.GameServer.World {

    public sealed class MutableWorld {
        public const ushort MapMaximumLayers = 15;
        public readonly MutableWorldNode Root;

        public MutableWorld(MutableWorldNode root) {
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            Root = root;
        }
    }
}