using System.Collections.Generic;

namespace COTS.GameServer.World.Loading {

    public static partial class WorldLoader {

        /// <summary>
        /// Array of 'ascii bytes' ['O', 'T', 'B', 'M'] or ['\0', '\0', '\0', '\0']
        /// </summary>
        private const int IdentifierLength = 4;

        /// <summary>
        /// NodeStart + NodeType + NodeEnd
        /// </summary>
        private const int MinimumNodeSize = 3;

        private const int MinimumWorldSize = IdentifierLength + MinimumNodeSize;

        private static ParsingNode ParseTree(ByteArrayReadStream stream) {
            // Skipping the first 4 bytes coz they are used to store a... identifier?
            stream.Skip(IdentifierLength);

            var firstMarker = (MarkupByte)stream.ReadByte();
            if (firstMarker != MarkupByte.Start)
                throw new MalformedWorldException();

            var guessedMaximumNodeDepth = 128;
            var nodeStack = new Stack<ParsingNode>(capacity: guessedMaximumNodeDepth);

            var rootNodeType = (byte)stream.ReadByte();
            var rootNode = new ParsingNode() {
                Type = (NodeType)rootNodeType,
                PropsBegin = (int)stream.Position
            };
            nodeStack.Push(rootNode);

            ParseTreeAfterRootNodeStart(stream, nodeStack);
            if (nodeStack.Count != 0)
                throw new MalformedWorldException();

            return rootNode;
        }

        private static void ParseTreeAfterRootNodeStart(
            ByteArrayReadStream stream,
            Stack<ParsingNode> nodeStack
            ) {
            while (!stream.IsOver) {
                var currentMark = (MarkupByte)stream.ReadByte();
                switch (currentMark) {
                    case MarkupByte.Start:
                    ProcessNodeStart(stream, nodeStack);
                    break;

                    case MarkupByte.End:
                    ProcessNodeEnd(stream, nodeStack);
                    break;

                    case MarkupByte.Escape:
                    ProcessNodeEscape(stream);
                    break;

                    default:
                    /// If it's not a <see cref="ParsingWorldNode.NodeMarker"/>, then it's just prop data
                    /// and we can safely skip it.
                    break;
                }
            }
        }

        private static void ProcessNodeStart(ByteArrayReadStream stream, Stack<ParsingNode> nodeStack) {
            if (!nodeStack.TryPeek(out var currentNode))
                throw new MalformedWorldException();

            if (currentNode.Children.Count == 0)
                currentNode.PropsEnd = stream.Position;

            var childType = stream.ReadByte();
            if (stream.IsOver)
                throw new MalformedWorldException();

            var child = new ParsingNode {
                Type = (NodeType)childType,
                PropsBegin = stream.Position//  + sizeof(MarkupByte)
            };

            currentNode.Children.Add(child);
            nodeStack.Push(child);
        }

        private static void ProcessNodeEnd(ByteArrayReadStream stream, Stack<ParsingNode> nodeStack) {
            if (!nodeStack.TryPeek(out var currentNode))
                throw new MalformedWorldException();

            if (currentNode.Children.Count == 0)
                currentNode.PropsEnd = stream.Position;

            nodeStack.Pop();
        }

        private static void ProcessNodeEscape(ByteArrayReadStream stream) {
            var escapedByte = stream.ReadByte();
            if (stream.IsOver)
                throw new MalformedWorldException();
        }
    }
}