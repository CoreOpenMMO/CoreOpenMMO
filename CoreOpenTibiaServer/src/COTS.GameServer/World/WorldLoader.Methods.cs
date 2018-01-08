using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace COTS.GameServer.World {

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

        public static World ParseWorld(byte[] serializedWorldData) {
            if (serializedWorldData == null)
                throw new ArgumentNullException(nameof(serializedWorldData));
            if (serializedWorldData.Length < MinimumWorldSize)
                throw new MalformedWorldException();

            using (var stream = new MemoryStream(serializedWorldData)) {
                var parsedTree = ParseTree(stream);
                var world = ConvertParsingTree(serializedWorldData, parsedTree);
                return world;
            }
        }

        private static ParsingWorldNode ParseTree(MemoryStream stream) {
            // Skipping the first 4 bytes coz they are used to store a... identifier?
            stream.Seek(IdentifierLength, SeekOrigin.Begin);

            var firstMarker = (ParsingWorldNode.NodeMarker)stream.ReadByte();
            if (firstMarker != ParsingWorldNode.NodeMarker.Start)
                throw new MalformedWorldException();

            var guessedMaximumNodeDepth = 128;
            var nodeStack = new Stack<ParsingWorldNode>(capacity: guessedMaximumNodeDepth);

            var rootNodeType = (byte)stream.ReadByte();
            var rootNode = new ParsingWorldNode() {
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
            MemoryStream stream,
            Stack<ParsingWorldNode> nodeStack
            ) {
            while (true) {
                var currentInt = stream.ReadByte();
                if (currentInt == -1)
                    return;

                var currentByte = (byte)currentInt;
                var currentMark = (ParsingWorldNode.NodeMarker)currentByte;
                switch (currentMark) {
                    case ParsingWorldNode.NodeMarker.Start:
                    ProcessNodeStart(stream, nodeStack);
                    break;

                    case ParsingWorldNode.NodeMarker.End:
                    ProcessNodeEnd(stream, nodeStack);
                    break;

                    case ParsingWorldNode.NodeMarker.Escape:
                    ProcessNodeEscape(stream);
                    break;

                    default:
                    /// If it's not a <see cref="ParsingWorldNode.NodeMarker"/>, then it's just prop data
                    /// and we can safely skip it.
                    break;
                }
            }
        }

        private static void ProcessNodeStart(MemoryStream stream, Stack<ParsingWorldNode> nodeStack) {
            if (!nodeStack.TryPeek(out var currentNode))
                throw new MalformedWorldException();

            if (currentNode.Children.Count == 0)
                currentNode.PropsEnd = (int)stream.Position;

            var childType = stream.ReadByte();
            if (childType == -1)
                throw new MalformedWorldException();

            var child = new ParsingWorldNode {
                Type = (byte)childType,
                PropsBegin = (int)stream.Position + sizeof(ParsingWorldNode.NodeMarker)
            };

            currentNode.Children.Add(child);
            nodeStack.Push(child);
        }

        private static void ProcessNodeEnd(MemoryStream stream, Stack<ParsingWorldNode> nodeStack) {
            if (!nodeStack.TryPeek(out var currentNode))
                throw new MalformedWorldException();

            if (currentNode.Children.Count == 0)
                currentNode.PropsEnd = (int)stream.Position;

            nodeStack.Pop();
        }

        private static void ProcessNodeEscape(MemoryStream stream) {
            var escapedByte = stream.ReadByte();
            if (stream.Position == stream.Length)
                throw new MalformedWorldException();
        }

        private static World ConvertParsingTree(byte[] serializedWorldData, ParsingWorldNode root) {
            if (serializedWorldData == null)
                throw new ArgumentNullException(nameof(serializedWorldData));
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            var worldRoot = ConvertingParsingNode(serializedWorldData, root);
            var world = new World(worldRoot);
            return world;
        }

        private static WorldNode ConvertingParsingNode(byte[] serializedWorldData, ParsingWorldNode subtree) {
            var propsSegment = new ArraySegment<byte>(
                array: serializedWorldData,
                offset: subtree.PropsBegin,
                count: subtree.PropsEnd - subtree.PropsBegin);

            var childen = subtree
                .Children
                .Select(c => ConvertingParsingNode(serializedWorldData, c))
                .ToArray();

            var immutableChildren = ReadOnlyArray<WorldNode>.WrapCollection(childen);

            Debug.Assert(propsSegment != null);
            Debug.Assert(childen != null);

            var worldNode = new WorldNode(
                props: propsSegment,
                children: immutableChildren
                );

            return worldNode;
        }
    }
}