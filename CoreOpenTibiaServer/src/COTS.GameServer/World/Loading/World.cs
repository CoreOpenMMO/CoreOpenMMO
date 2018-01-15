using System;

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

        public World(WorldNode root, byte[] serializedWorldData) {
            if (root == null)
                throw new ArgumentNullException(nameof(root));
            if (serializedWorldData == null)
                throw new ArgumentNullException(nameof(serializedWorldData));

            this.Root = root;
            this.SerializedWorldData = serializedWorldData;

            // Okay, let's check if the world we loaded is supported
            var header = this.GetWorldRootNodeHeader();

            // Checking supported world encoding versions
            if (header.WorldEncodingVersion < World.Encoding.SupportedVersion)
                throw new UnsupportedWorldEncodingVersion();

            // Checking supported item encoding versions
            if (header.ItemEncodingMajorVersion < Items.Encoding.SupportedMajorVersion)
                throw new UnsupportedItemEncodingVersion();
            if (header.ItemEncodingMinorVersion < Items.Encoding.SupportedMinorVersion)
                throw new UnsupportedItemEncodingVersion();
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

        public void ParseWorldData() {
            var worldDataNode = this.Root.Children[0];

            var rawStream = new ByteArrayReadStream(
                array: SerializedWorldData,
                position: worldDataNode.PropsBegin);
            var serializationStream = new WorldSerializationReadStream(rawStream);

            string worldDescription = null;

            while (!serializationStream.IsOver) {
                var attribute = (NodeAttribute)serializationStream.ReadByte();
                switch (attribute) {
                    case NodeAttribute.WorldDescription: {
                            if(worldDescription != null) {
                                throw new MalformedWorldHeaderNodeException("Multiple world description attributes.");
                            }
                        }
                    break;

                    case NodeAttribute.ExtensionFileForSpawns:
                    throw new NotImplementedException();
                    break;

                    case NodeAttribute.ExtensionFileForHouses:
                    throw new NotImplementedException();
                    break;

                    default:
                    throw new MalformedWorldHeaderNodeException();
                }
            }
        }
    }
}