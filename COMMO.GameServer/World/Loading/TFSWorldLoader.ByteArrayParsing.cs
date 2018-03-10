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

		private static OTBTree ExtractOTBTree(byte[] serializedWorldData) {
			var stream = new ByteArrayReadStream(serializedWorldData);

			// Skipping the first 4 bytes coz they are used to store a... identifier?
			stream.Skip(IdentifierLength);

			var firstMarker = (OTBMarkupByte)stream.ReadByte();
			if (firstMarker != OTBMarkupByte.Start)
				throw new MalformedWorldException();

			var guessedMaximumNodeDepth = 128;
			var nodeStack = new Stack<OldOTBNode>(capacity: guessedMaximumNodeDepth);

			var rootNodeType = (byte)stream.ReadByte();
			var rootNode = new OldOTBNode() {
				Type = (OTBNodeType)rootNodeType,
				DataBegin = (int)stream.Position
			};
			nodeStack.Push(rootNode);

			ParseTreeAfterRootNodeStart(stream, nodeStack);
			if (nodeStack.Count != 0)
				throw new MalformedWorldException();

			return new OTBTree(data: serializedWorldData, root: rootNode);
		}

		private static void ParseTreeAfterRootNodeStart(
			ByteArrayReadStream stream,
			Stack<OldOTBNode> nodeStack
			) {
			while (!stream.IsOver) {
				var currentMark = (OTBMarkupByte)stream.ReadByte();
				if (currentMark < OTBMarkupByte.Escape) {
					/// Since <see cref="OTBMarkupByte"/> can only have values Escape (0xFD), Start (0xFE) and
					/// End (0xFF), if currentMark < Escape, then it's just prop data 
					/// and we can safely skip it.
					continue;
				}

				switch (currentMark) {
					case OTBMarkupByte.Start:
					ProcessNodeStart(stream, nodeStack);
					break;

					case OTBMarkupByte.End:
					ProcessNodeEnd(stream, nodeStack);
					break;

					case OTBMarkupByte.Escape:
					ProcessNodeEscape(stream);
					break;

					default:
					throw new InvalidOperationException();
				}
			}
		}

		private static void ProcessNodeStart(ByteArrayReadStream stream, Stack<OldOTBNode> nodeStack) {
			if (!nodeStack.TryPeek(out var currentNode))
				throw new MalformedWorldException();

			if (currentNode.Children.Count == 0)
				currentNode.DataEnd = stream.Position;

			var childType = stream.ReadByte();
			if (stream.IsOver)
				throw new MalformedWorldException();

			var child = new OldOTBNode {
				Type = (OTBNodeType)childType,
				DataBegin = stream.Position//  + sizeof(MarkupByte)
			};

			currentNode.Children.Add(child);
			nodeStack.Push(child);
		}

		private static void ProcessNodeEnd(ByteArrayReadStream stream, Stack<OldOTBNode> nodeStack) {
			if (!nodeStack.TryPeek(out var currentNode))
				throw new MalformedWorldException();

			if (currentNode.Children.Count == 0)
				currentNode.DataEnd = stream.Position;

			nodeStack.Pop();
		}

		private static void ProcessNodeEscape(ByteArrayReadStream stream) {
			var escapedByte = stream.ReadByte();
			if (stream.IsOver)
				throw new MalformedWorldException();
		}
	}
}