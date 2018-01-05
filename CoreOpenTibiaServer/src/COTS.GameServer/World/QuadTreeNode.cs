namespace COTS.GameServer.World {

    public class QuadTreeNode {
        public readonly QuadTreeNode[] Children = new QuadTreeNode[4];

        private const ushort IndexingHackeryAndValue = 0b_1000_0000_0000_0000;
        private const ushort XShiftRightInitialValue = 15;
        private const ushort YShifyRightInitialValue = 14;

        public readonly bool IsLeaf;

        protected QuadTreeNode(bool isLeaf) {
            IsLeaf = isLeaf;
        }

        public QuadTreeNode()
            : this(isLeaf: false) { }

        public QuadTreeLeafNode GetLeaf(uint x, uint y) {
            if (IsLeaf) {
                return this as QuadTreeLeafNode;
            }

            var node = Children[ComputeChildrenIndex(x, y)];
            if (node == null)
                return null;
            else
                return node.GetLeaf(x << 1, y << 1);
        }

        public QuadTreeLeafNode CreateLeaf(uint x, uint y, uint level) {
            if (IsLeaf)
                return this as QuadTreeLeafNode;

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

            // Add comment to explain the *2 and -1
            return Children[childIndex].CreateLeaf(x * 2, y * 2, level - 1);
        }

#warning Port `static Leaf getLeafStatic(Node node, uint32_t x, uint32_t y)'

        private static uint ComputeChildrenIndex(uint x, uint y) {
            // I know, right?
            return
                ((x & IndexingHackeryAndValue) >> XShiftRightInitialValue)
                |
                ((y & IndexingHackeryAndValue) >> YShifyRightInitialValue);
        }
    }
}