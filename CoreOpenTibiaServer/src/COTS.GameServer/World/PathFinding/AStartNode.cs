using System;

namespace COTS.GameServer.World.PathFinding {

    public sealed class AStartNode : IEquatable<AStartNode> {
        public readonly int X;
        public readonly int Y;
        private readonly int _hashCode;

        public AStartNode Parent;

        public int G;
        public readonly int H;

        public AStartNode(
            int x,
            int y,
            AStartNode parent,
            int g,
            int h
            ) {
            X = x;
            Y = y;
            Parent = parent;
            G = g;
            H = h;

            _hashCode = HashHelper.Start
                .CombineHashCode(X)
                .CombineHashCode(Y);
        }

        public int Priority => G + H;
        public int QueueIndex;

        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj) {
            throw new NotImplementedException("Don't call the non generic version");
        }

        public bool Equals(AStartNode other) {
            // Not checking for nulls for peprformance reasons
            return this.X == other.X &&
                this.Y == other.Y;
        }

        public PathCoordinate ToPathCoordinate() {
            return new PathCoordinate(x: X, y: Y);
        }
    }
}