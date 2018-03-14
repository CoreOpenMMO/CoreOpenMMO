using System;
using System.Collections.Generic;
using COMMO.GameServer.OTBParsing;

namespace COMMO.GameServer.World.Loading {

	public static partial class TFSWorldLoader {

		/// <summary>
		/// Array of 'ascii bytes' ['O', 'T', 'B', 'M'] or ['\0', '\0', '\0', '\0']
		/// </summary>
		private const int IdentifierLength = 4;

		/// <summary>
		/// NodeStart + NodeType + NodeEnd
		/// </summary>
		private const int MinimumNodeSize = 3;

		private const int MinimumWorldSize = IdentifierLength + MinimumNodeSize;

		private static OTBNode ExtractOTBTree(byte[] serializedWorldData) {
			var stream = new ReadOnlyMemoryStream(serializedWorldData);

			// Skipping the first 4 bytes coz they are used to store a... identifier?
			stream.Skip(IdentifierLength);

			var treeBuilder = new OTBTreeBuilder(serializedWorldData);
			while (!stream.IsOver) {
				var currentMark = (OTBMarkupByte)stream.ReadByte();
				if (currentMark < OTBMarkupByte.Escape) {
					/// Since <see cref="OTBMarkupByte"/> can only have values Escape (0xFD), Start (0xFE) and
					/// End (0xFF), if currentMark < Escape, then it's just prop data 
					/// and we can safely skip it.
					continue;
				}

				var nodeType = (OTBNodeType)stream.ReadByte();

				switch (currentMark) {
					case OTBMarkupByte.Start:
					treeBuilder.AddNodeDataBegin(
						start: stream.Position,
						type: nodeType);
					break;

					case OTBMarkupByte.End:
					treeBuilder.AddNodeEnd(stream.Position);
					break;

					case OTBMarkupByte.Escape:
					stream.Skip();
					break;

					default:
					throw new InvalidOperationException();
				}
			}

			return treeBuilder.BuildTree();
		}
	}
}