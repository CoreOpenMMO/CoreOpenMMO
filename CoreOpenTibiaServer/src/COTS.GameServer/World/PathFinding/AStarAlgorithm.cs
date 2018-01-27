using System;
using System.Collections.Generic;

namespace COTS.GameServer.World.PathFinding {

    public static class AStarAlgorithm {
        public const int MaximumNodes = 512;
        public const int NormalWalkCost = 10;
        public const int DiagonalWalkCost = 25;

        public enum ComputePathResult {
            NoNeedToMove,
            MaximumNumberOfNodesReached,
            NoPathFound,
            PathFound
        }

        public static ComputePathResult TryComputePath(Position start, Position end, out List<PathCoordinate> path) {
            // Trivial case
            if (start == end) {
                path = null;
                return ComputePathResult.NoNeedToMove;
            }

            // Initializing open and closed lists
            var openList = new PriorityQueue(MaximumNodes);
            var closedList = new HashSet<AStartNode>();
            openList.TryEnqueue(new AStartNode(
                            x: start.X,
                            y: start.Y,
                            parent: null,
                            g: 0,
                            h: Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y)
                            ));

            while (openList.TryDequeue(out var currentNode)) {
                foreach (var sucessor in GenerateNeighbors(currentNode, end)) {
                    // We check whether we reached the goal before checking if
                    // we already fully explored this node (i.e.: if this node
                    // is in the closed list) because it's cheaper to do so.
                    if (IsGoal(sucessor, end)) {
                        path = GeneratePath(sucessor);
                        return ComputePathResult.PathFound;
                    }

                    // If the node is in the closed list, we already fully explored it
                    // and we can safely go check the next neighbor
                    if (closedList.Contains(sucessor))
                        continue;

                    // If this node is not already queued to be explore,
                    // try adding it to the queue
                    if (!openList.Contains(sucessor)) {
                        if (!openList.TryEnqueue(sucessor)) {
                            path = null;
                            return ComputePathResult.MaximumNumberOfNodesReached;
                        }
                    } else {
                        // If the node is already queued to be explored, and we reached
                        // it again, we must check if the path we took to reach it
                        // this time is better than the one we took previously
                        UpdateNodeIfNecessary(openList, sucessor, currentNode);
                    }
                }

                // Since we have explored all this nodes, we must add "mark" it
                // as "fully explored"
                closedList.Add(currentNode);
            }

            path = null;
            return ComputePathResult.NoPathFound;
        }

        private static void UpdateNodeIfNecessary(PriorityQueue priorityQueue, AStartNode node, AStartNode potentialNewParent) {
            int newGCost;
            if (node.X != potentialNewParent.X &&
                node.Y != potentialNewParent.Y) {
                newGCost = potentialNewParent.G + DiagonalWalkCost;
            } else {
                newGCost = potentialNewParent.G + NormalWalkCost;
            }

            if (newGCost >= node.G) {
                // New cost is not an improvement
                return;
            }

            priorityQueue.UpdateNodeGCost(node, newGCost);
        }

        private static bool IsGoal(AStartNode s, in Position end) {
            return s.X == end.X && s.Y == end.Y;
        }

        private static IEnumerable<AStartNode> GenerateNeighbors(AStartNode parent, in Position goal) {
            // This method depends on the World class to check whether a given
            // point is "walkable" or not
            throw new NotImplementedException();
        }

        private static List<PathCoordinate> GeneratePath(AStartNode end) {
            // We "over-allocate" memory to prevent resizing the list
            var maximumPathLength = end.G * end.G;
            var path = new List<PathCoordinate>(maximumPathLength);

            var currentNode = end;
            while (currentNode != null) {
                path.Add(currentNode.ToPathCoordinate());
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }
    }
}