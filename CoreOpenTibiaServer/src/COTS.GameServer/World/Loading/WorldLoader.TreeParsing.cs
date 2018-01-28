using System;
using System.Collections.Generic;
using COTS.GameServer.OTBParsing;

namespace COTS.GameServer.World.Loading {

    public static partial class WorldLoader {

        public static WorldHeader GetWorldHeader(ParsingTree tree) {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));

            var parsingStream = new ParsingStream(tree, tree.Root);

            var worldEncodingVersion = parsingStream.ReadUInt32();
            var worldWidth = parsingStream.ReadUInt16();
            var worldHeight = parsingStream.ReadUInt16();
            var itemEncodingMajorVersion = parsingStream.ReadUInt32();
            var itemEncodingMinorVersion = parsingStream.ReadUInt32();

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

            var parsingStream = new ParsingStream(tree, worldDataNode);

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