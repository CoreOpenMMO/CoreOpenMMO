using System;
using System.Collections.Generic;

namespace COTS.GameServer.World.Loading {

    public static partial class WorldLoader {

        public static WorldHeader GetWorldHeader(ParsingTree tree) {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));

            var parsingStream = new WorldParsingStream(tree, tree.Root);

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

            var parsingStream = new WorldParsingStream(tree, worldDataNode);

            var worldDescription = new List<string>();
            string spawnsFilename = null;
            string housesFilename = null;

            while (!parsingStream.IsOver) {
                var attribute = (NodeAttribute)parsingStream.ReadByte();
                switch (attribute) {
                    case NodeAttribute.WorldDescription:
                    worldDescription.Add(parsingStream.ReadString());
                    break;

                    case NodeAttribute.ExtensionFileForSpawns:
                    if (spawnsFilename != null) {
                        throw new MalformedWorldAttributesNodeException("Multiple filenames for world spawns.");
                    } else {
                        spawnsFilename = parsingStream.ReadString();
                    }
                    break;

                    case NodeAttribute.ExtensionFileForHouses:
                    if (housesFilename != null) {
                        throw new MalformedWorldAttributesNodeException("Multiple filenames for world houses.");
                    } else {
                        housesFilename = parsingStream.ReadString();
                    }
                    break;

                    default:
                    throw new MalformedWorldAttributesNodeException("Unknown attribute found in world attributes note.");
                }
            }

            var formattedWorldDescription = string.Join(
                separator: Environment.NewLine,
                values: worldDescription);

            return new WorldAttributes(
                worldDescription: formattedWorldDescription,
                spawnsFilename: spawnsFilename,
                housesFilename: housesFilename);
        }
    }
}