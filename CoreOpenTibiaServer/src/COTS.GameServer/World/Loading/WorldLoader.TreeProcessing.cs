using System;

namespace COTS.GameServer.World.Loading {

    public static partial class WorldLoader {

        private static WorldHeader GetWorldRootNodeHeader(byte[] serializedWorldData, WorldNode root) {
            var rawStream = new ByteArrayReadStream(
                array: serializedWorldData,
                position: root.PropsBegin);
            var parsingStream = new WorldParsingStream(rawStream);

            UInt32 worldEncodingVersion = parsingStream.ReadUInt32();
            UInt16 worldWidth = parsingStream.ReadUInt16();
            UInt16 worldHeight = parsingStream.ReadUInt16();
            UInt32 itemEncodingMajorVersion = parsingStream.ReadUInt32();
            UInt32 itemEncodingMinorVersion = parsingStream.ReadUInt32();

            return new WorldHeader(
                worldEncodingVersion: worldEncodingVersion,
                worldWidth: worldWidth,
                worldHeight: worldHeight,
                itemEncodingMajorVersion: itemEncodingMajorVersion,
                itemEncodingMinorVersion: itemEncodingMinorVersion);
        }

        private static WorldAttributes ParseWorldData(byte[] serializedWorldData, WorldNode root) {
            throw new NotImplementedException();

            //var worldDataNode = this.Root.Children[0];

            //var rawStream = new ByteArrayReadStream(
            //    array: SerializedWorldData,
            //    position: worldDataNode.PropsBegin);
            //var serializationStream = new WorldSerializationReadStream(rawStream);

            //string worldDescription = null;

            //while (!serializationStream.IsOver) {
            //    var attribute = (NodeAttribute)serializationStream.ReadByte();
            //    switch (attribute) {
            //        case NodeAttribute.WorldDescription: {
            //                if (worldDescription != null) {
            //                    throw new MalformedWorldHeaderNodeException("Multiple world description attributes.");
            //                }
            //            }
            //            break;

            //        case NodeAttribute.ExtensionFileForSpawns:
            //        throw new NotImplementedException();
            //        break;

            //        case NodeAttribute.ExtensionFileForHouses:
            //        throw new NotImplementedException();
            //        break;

            //        default:
            //        throw new MalformedWorldHeaderNodeException();
            //    }
            //}
        }
    }
}