using System;

namespace COTS.GameServer.World {

    public class QuadTreeNode {
        public readonly QuadTreeNode TopLeftChild;
        public readonly QuadTreeNode TopRightChild;
        public readonly QuadTreeNode BottomRightChild;
        public readonly QuadTreeNode BottomLeftChild;

        public QuadTreeNode(
            QuadTreeNode topLeftChild,
            QuadTreeNode topRightChild,
            QuadTreeNode bottomRightChild,
            QuadTreeNode bottomLeftChild
            ) {
            if (topLeftChild == null)
                throw new ArgumentNullException(nameof(topLeftChild));
            if (topRightChild == null)
                throw new ArgumentNullException(nameof(topRightChild));
            if (bottomRightChild == null)
                throw new ArgumentNullException(nameof(bottomRightChild));
            if (bottomLeftChild == null)
                throw new ArgumentNullException(nameof(bottomLeftChild));

            TopLeftChild = topLeftChild;
            TopRightChild = topRightChild;
            BottomRightChild = bottomRightChild;
            BottomLeftChild = bottomLeftChild;
            throw new NotImplementedException();
        }

        public bool TryGetLeaf(int x, int z) {
            throw new NotImplementedException();
        }
    }
}