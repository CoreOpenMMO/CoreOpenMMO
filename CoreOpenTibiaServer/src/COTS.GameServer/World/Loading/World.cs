namespace COTS.GameServer.World.Loading {

    public sealed class World {
        public const ushort MapMaximumLayers = 15;

        public static class Encoding {
            public const uint SupportedVersion = 2;
        }

        /// <summary>
        /// To save memory, the <see cref="WorldNode"/>s don't actually store their information.
        /// Instead, they just keep a `offset' and a `byte count' to this array.
        /// </summary>
        public readonly byte[] SerializedWorldData;

        /// <summary>
        /// World info is store in a "adhoc xml".
        /// This is the root node of such pseudo-xmml.
        /// </summary>
        public readonly WorldNode Root;
    }
}