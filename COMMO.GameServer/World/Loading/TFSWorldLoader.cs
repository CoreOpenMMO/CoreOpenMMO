using COMMO.GameServer.OTBParsing;
using System;
using System.Collections.Generic;

namespace COMMO.GameServer.World.Loading {

	public static partial class TFSWorldLoader {

		public static World ParseWorld(byte[] serializedWorldData) {
			if (serializedWorldData.Length < MinimumWorldSize)
				throw new MalformedWorldException();

			var otbTree = ExtractOTBTree(serializedWorldData);
			throw new NotImplementedException();
		}

		public static TFSWorldHeader GetWorldHeader(OldOTBTree tree) {
			if (tree == null)
				throw new ArgumentNullException(nameof(tree));

			var parsingStream = new OTBNodeParsingStream(tree, tree.Root);

			var worldEncodingVersion = parsingStream.ReadUInt32();
			var worldWidth = parsingStream.ReadUInt16();
			var worldHeight = parsingStream.ReadUInt16();
			var itemEncodingMajorVersion = parsingStream.ReadUInt32();
			var itemEncodingMinorVersion = parsingStream.ReadUInt32();

			return new TFSWorldHeader(
				worldEncodingVersion: worldEncodingVersion,
				worldWidth: worldWidth,
				worldHeight: worldHeight,
				itemEncodingMajorVersion: itemEncodingMajorVersion,
				itemEncodingMinorVersion: itemEncodingMinorVersion);
		}

		public static TFSWorldAttributes GetWorldAttributes(OldOTBTree tree) {
			if (tree == null)
				throw new ArgumentNullException(nameof(tree));

			if (tree.Root.Children.Count != 1)
				throw new MalformedWorldException();

			var worldDataNode = tree.Root.Children[0];
			if ((OTBNodeType)worldDataNode.Type != OTBNodeType.WorldData)
				throw new MalformedWorldException();

			var parsingStream = new OTBNodeParsingStream(tree, worldDataNode);

			var worldDescription = new List<string>();
			string spawnsFilename = null;
			string housesFilename = null;

			while (!parsingStream.IsOver) {
				var attribute = (TFSWorldNodeAttribute)parsingStream.ReadByte();
				switch (attribute) {
					case TFSWorldNodeAttribute.WorldDescription:
					worldDescription.Add(parsingStream.ReadString());
					break;

					case TFSWorldNodeAttribute.ExtensionFileForSpawns:
					if (spawnsFilename != null) {
						throw new MalformedWorldAttributesNodeException("Multiple filenames for world spawns.");
					} else {
						spawnsFilename = parsingStream.ReadString();
					}
					break;

					case TFSWorldNodeAttribute.ExtensionFileForHouses:
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

			return new TFSWorldAttributes(
				worldDescription: formattedWorldDescription,
				spawnsFilename: spawnsFilename,
				housesFilename: housesFilename);
		}
	}
}