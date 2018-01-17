using System;

namespace COTS.GameServer.World.Loading {

    public static partial class WorldLoader {

        public static WorldHeader GetWorldHeader(ParsingTree tree) {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));

            var rawStream = new ByteArrayReadStream(
                array: tree.Data,
                position: tree.Root.PropsBegin);
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

        public static WorldAttributes GetWorldAttributes(ParsingTree tree) {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));

            if (tree.Root.Children.Count != 1)
                throw new MalformedWorldException();

            var worldDataNode = tree.Root.Children[0];
            if ((NodeType)worldDataNode.Type != NodeType.WorldData)
                throw new MalformedWorldException();

            var rawStream = new ByteArrayReadStream(
                array: tree.Data,
                position: worldDataNode.PropsBegin);
            var parsingStream = new WorldParsingStream(rawStream);

            string worldDescription = null;
            string spawnsFilename = null;
            string housesFilename = null;

            while (!parsingStream.IsOver) {
                var attribute = (NodeAttribute)parsingStream.ReadByte();
                switch (attribute) {
                    case NodeAttribute.WorldDescription:
                    if (worldDescription != null) {
                        throw new MalformedAttributesNodeException("Multiple world description attributes.");
                    } else {
                        worldDescription = parsingStream.ReadString();
                    }
                    break;

                    case NodeAttribute.ExtensionFileForSpawns:
                    if (spawnsFilename != null) {
                        throw new MalformedAttributesNodeException("Multiple filenames for world spawns.");
                    } else {
                        spawnsFilename = parsingStream.ReadString();
                    }
                    break;

                    case NodeAttribute.ExtensionFileForHouses:
                    if (housesFilename != null) {
                        throw new MalformedAttributesNodeException("Multiple filenames for world houses.");
                    } else {
                        housesFilename = parsingStream.ReadString();
                    }
                    break;

                    default:
                    throw new MalformedAttributesNodeException("Unknown attribute found in world attributes note.");
                }
            }

            return new WorldAttributes(
                description: worldDescription,
                spawnsFilename: spawnsFilename,
                housesFilename: housesFilename);
        }
    }
}