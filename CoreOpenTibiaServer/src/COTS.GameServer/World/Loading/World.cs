using System;

namespace COTS.GameServer.World.Loading {

    public sealed class World {
        public const ushort MapMaximumLayers = 15;

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

        public World(WorldNode root , byte[] serializedWorldData) {
            if (root == null)
                throw new ArgumentNullException(nameof(root));
            if (serializedWorldData == null)
                throw new ArgumentNullException(nameof(serializedWorldData));

            Root = root;
            SerializedWorldData = serializedWorldData;
        }

        public WorldRootNodeHeader GetWorldRootNodeHeader() {
            var byteArrayStream = new ByteArrayReadStream(
                array: SerializedWorldData,
                position: Root.PropsBegin);
            var serializationStream = new WorldSerializationReadStream(byteArrayStream);


            UInt32 worldEncodingVersion = serializationStream.ReadUInt32();
            UInt16 worldWidth = serializationStream.ReadUInt16();
            UInt16 worldHeight = serializationStream.ReadUInt16();
            UInt32 itemEncodingMajorVersion = serializationStream.ReadUInt32();
            UInt32 itemEncodingMinorVersion = serializationStream.ReadUInt32();

            return new WorldRootNodeHeader(
                worldEncodingVersion: worldEncodingVersion,
                worldWidth: worldWidth,
                worldHeight: worldHeight,
                itemEncodingMajorVersion: itemEncodingMajorVersion,
                itemEncodingMinorVersion: itemEncodingMinorVersion);
        }
    }
}