using System;
using System.Collections.Generic;
using System.IO;

namespace COTS.GameServer.World {

    public sealed class WorldLoader {
        public readonly byte[] SerializedWorldData;
        public readonly WorldNode RootNode;
        public readonly List<byte> PropBuffer;

        public static WorldNode ParseTree(byte[] world) {
            if (world == null)
                throw new ArgumentNullException(nameof(world));
            if (world.Length == 0)
                throw new MalformedWorldException();
#warning Add more format checks

#error Fix the last byte reading issue
            using (var stream = new MemoryStream(world)) {
                var firstMarker = (WorldNode.NodeMarker)stream.ReadByte();
                firstMarker = (WorldNode.NodeMarker)stream.ReadByte();
                firstMarker = (WorldNode.NodeMarker)stream.ReadByte();
                firstMarker = (WorldNode.NodeMarker)stream.ReadByte();
                firstMarker = (WorldNode.NodeMarker)stream.ReadByte();
                if (firstMarker != WorldNode.NodeMarker.Start)
                    throw new MalformedWorldException();

                var guessedNodeCount = world.Length / 4;
                var nodeStack = new Stack<WorldNode>(capacity: guessedNodeCount);

                var rootNodeType = (byte)stream.ReadByte();
                var rootNodeFirstContentByte = (byte)stream.ReadByte();
                var rootNode = new WorldNode() {
                    Type = rootNodeType,
                    PropsBegin = (int)stream.Position
                };
                nodeStack.Push(rootNode);

                ParseTree(stream, nodeStack, rootNodeFirstContentByte);
                if (nodeStack.Count != 0)
                    throw new MalformedWorldException();

                return rootNode;
            }
        }

        private static void ParseTree(
            MemoryStream stream,
            Stack<WorldNode> nodeStack,
            byte rootNodeFirstContentByte
            ) {
            var currentByte = rootNodeFirstContentByte;

            while (stream.Position != stream.Length ) {
                var markerType = (WorldNode.NodeMarker)currentByte;
                switch (markerType) {
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
                    /// If it's not a <see cref="WorldNode.NodeMarker"/>, then it's just prop data
                    /// and we can safely skip it.
                    break;
                }

                currentByte = (byte)stream.ReadByte();
            }
        }

        private static void ProcessNodeStart(MemoryStream stream, Stack<WorldNode> nodeStack) {
            if (!nodeStack.TryPeek(out var currentNode))
                throw new MalformedWorldException();

            if (currentNode.Children.Count == 0)
                currentNode.PropsEnd = (int)stream.Position;

            var childType = stream.ReadByte();
            if (childType == -1)
                throw new MalformedWorldException();

            var child = new WorldNode {
                Type = (byte)childType,
                PropsBegin = (int)stream.Position + sizeof(WorldNode.NodeMarker)
            };

            nodeStack.Push(child);
        }

        private static void ProcessNodeEnd(MemoryStream stream, Stack<WorldNode> nodeStack) {
            if (!nodeStack.TryPeek(out var currentNode))
                throw new MalformedWorldException();

            if (currentNode.Children.Count == 0)
                currentNode.PropsEnd = (int)stream.Position;

            nodeStack.Pop();
        }

        private static void ProcessNodeEscape(MemoryStream stream) {
            var scapedByte = stream.ReadByte();
            if (stream.Position == stream.Length)
                throw new MalformedWorldException();
        }

        public PropStream GetProps(WorldNode node) {
            throw new NotImplementedException();
        }
    }
}