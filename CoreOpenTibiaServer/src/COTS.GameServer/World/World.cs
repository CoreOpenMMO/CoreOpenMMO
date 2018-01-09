using System;

namespace COTS.GameServer.World {

    public sealed class World {
        public const ushort MapMaximumLayers = 15;
        public readonly WorldNode Root;

        public World(WorldNode root) {
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            Root = root;
        }
    }
}