namespace COTS.GameServer.World {

    public class QuadTreeNode {
        public readonly QuadTreeNode[] Children = new QuadTreeNode[4];

        private const ushort IndexingHackeryAndValue = 0b1000_0000_0000_0000;
        private const ushort XShiftRightInitialValue = 15;
        private const ushort YShifyRightInitialValue = 14;

        public readonly bool IsLeaf;

        protected QuadTreeNode(bool isLeaf) {
            IsLeaf = isLeaf;
        }

        public QuadTreeNode()
            : this(isLeaf: false) { }

        public bool TryGetLeaf(uint x, uint y, out QuadTreeLeafNode leaf) {
            if (IsLeaf) {
                leaf = (QuadTreeLeafNode)this;
                return true;
            }

            var node = Children[ComputeChildrenIndex(x, y)];
            if(node != null) 
                return node.TryGetLeaf(x << 1, y << 1, out leaf);

            // Node not created yet
            leaf = null;
            return false;
        }

        public QuadTreeLeafNode CreateLeafOrGetReference(uint x, uint y, uint level) {
            if (IsLeaf)
                return (QuadTreeLeafNode)this;

            var childIndex = ComputeChildrenIndex(x, y);

            if (Children[childIndex] == null) {
                // Oughta create a new child
                if (level == Floor.FloorBits) {
                    Children[childIndex] = new QuadTreeLeafNode();
                } else {
                    Children[childIndex] = new QuadTreeNode();
                }
            } else {
                // Child is already not null, no need to do anything
            }

            return Children[childIndex].CreateLeafOrGetReference(x << 1, y << 1, level - 1);
        }

        private static uint ComputeChildrenIndex(uint x, uint y) {
            // I know, right?
            return
                ((x & IndexingHackeryAndValue) >> XShiftRightInitialValue)
                |
                ((y & IndexingHackeryAndValue) >> YShifyRightInitialValue);
        }
    }
}