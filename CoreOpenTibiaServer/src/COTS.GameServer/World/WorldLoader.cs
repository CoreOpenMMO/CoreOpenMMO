using System;
using System.Collections.Generic;

namespace COTS.GameServer.World {

    public static class WorldLoader {

        /// <summary>
        /// Array of 'ascii bytes' ['O', 'T', 'B', 'M'] or ['\0', '\0', '\0', '\0']
        /// </summary>
        private const int IdentifierLength = 4;

        /// <summary>
        /// NodeStart + NodeType + NodeEnd
        /// </summary>
        private const int MinimumNodeSize = 3;

        private const int MinimumWorldSize = IdentifierLength + MinimumNodeSize;

        public static World ParseWorld(byte[] serializedWorldData) {
            if (serializedWorldData == null)
                throw new ArgumentNullException(nameof(serializedWorldData));
            if (serializedWorldData.Length < MinimumWorldSize)
                throw new MalformedWorldException();

            var stream = new ByteArrayReadStream(serializedWorldData);
            var rootNode = ParseTree(stream);
            var world = new World(
                root: rootNode,
                serializedWorldData: serializedWorldData);

            return world;
        }

        private static WorldNode ParseTree(ByteArrayReadStream stream) {
            // Skipping the first 4 bytes coz they are used to store a... identifier?
            stream.Skip(IdentifierLength);

            var firstMarker = (WorldNode.NodeMarker)stream.ReadByte();
            if (firstMarker != WorldNode.NodeMarker.Start)
                throw new MalformedWorldException();

            var guessedMaximumNodeDepth = 128;
            var nodeStack = new Stack<WorldNode>(capacity: guessedMaximumNodeDepth);

            var rootNodeType = (byte)stream.ReadByte();
            var rootNode = new WorldNode() {
                Type = rootNodeType,
                PropsBegin = (int)stream.Position + 1
            };
            nodeStack.Push(rootNode);

            ParseTreeAfterRootNodeStart(stream, nodeStack);
            if (nodeStack.Count != 0)
                throw new MalformedWorldException();

            return rootNode;
        }

        private static void ParseTreeAfterRootNodeStart(
            ByteArrayReadStream stream,
            Stack<WorldNode> nodeStack
            ) {
            while (true) {
                if (stream.IsOver)
                    return;

                var currentByte = stream.ReadByte();
                var currentMark = (WorldNode.NodeMarker)currentByte;
                switch (currentMark) {
                    case WorldNode.NodeMarker.Start:
                    ProcessNodeStart(stream, nodeStack);
                    break;

                    case WorldNode.NodeMarker.End:
                    ProcessNodeEnd(stream, nodeStack);
                    break;

                    case WorldNode.NodeMarker.Escape:
                    ProcessNodeEscape(stream);
                    break;

                    default:
                    /// If it's not a <see cref="ParsingWorldNode.NodeMarker"/>, then it's just prop data
                    /// and we can safely skip it.
                    break;
                }
            }
        }

        private static void ProcessNodeStart(ByteArrayReadStream stream, Stack<WorldNode> nodeStack) {
            if (!nodeStack.TryPeek(out var currentNode))
                throw new MalformedWorldException();

            if (currentNode.Children.Count == 0)
                currentNode.PropsEnd = stream.Position;

            var childType = stream.ReadByte();
            if (stream.IsOver)
                throw new MalformedWorldException();

            var child = new WorldNode {
                Type = childType,
                PropsBegin = stream.Position + sizeof(WorldNode.NodeMarker)
            };

            currentNode.Children.Add(child);
            nodeStack.Push(child);
        }

        private static void ProcessNodeEnd(ByteArrayReadStream stream, Stack<WorldNode> nodeStack) {
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