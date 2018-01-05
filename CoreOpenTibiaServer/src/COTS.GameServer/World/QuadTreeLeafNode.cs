namespace COTS.GameServer.World {

    public sealed class QuadTreeLeafNode : QuadTreeNode {
        public readonly QuadTreeLeafNode SouthNeighbor;
        public readonly QuadTreeLeafNode EastNeighbor;
        public readonly ReadOnlyArray<Floor> Floors;

        public QuadTreeLeafNode()
            : base(isLeaf: true) { }
    }
}