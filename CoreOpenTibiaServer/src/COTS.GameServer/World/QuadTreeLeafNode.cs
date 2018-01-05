using System;

namespace COTS.GameServer.World {

    public sealed class QuadTreeLeafNode : QuadTreeNode {

        public readonly QuadTreeLeafNode SouthNeighbor;
        public readonly QuadTreeLeafNode EastNeighbor;
        public readonly ReadOnlyArray<Floor> Floors;

        private QuadTreeLeafNode(
            QuadTreeNode topLeftChild,
            QuadTreeNode topRightChild,
            QuadTreeNode bottomRightChild,
            QuadTreeNode bottomLeftChild
            ) : base(
                topLeftChild,
                topRightChild,
                bottomRightChild,
                bottomLeftChild
                ) {

            throw new NotImplementedException();
        }     
    }
}