namespace COMMO.GameServer.World {

	public sealed class QuadTree {
		private const int SignalBit = unchecked((int)0b10000000_00000000_00000000_00000000);
		private const int XRightShift = 31;
		private const int YRightShift = 30;
		private const int RootZoomLevel = 31;
		private const int LeafZoomLevel = 3;

		private readonly QuadTreeNode _root = new QuadTreeNode();

		public bool TryGetLeaf(int x, int y, out QuadTreeLeafNode leaf) {
			var node = _root;

			while (node != null && !node.IsLeaf)
				node = node.Children[ComputeChildrenIndex(x, y)];

			leaf = (QuadTreeLeafNode)node;
			return node != null;
		}

		public QuadTreeLeafNode CreateLeafOrGetReference(int x, int y) {
			var currentNode = _root;
			var zoomLevel = RootZoomLevel;

			while (!currentNode.IsLeaf) {
				var childIndex = ComputeChildrenIndex(x, y);

				// If we encounter a null, that means that this part of
				// the world haven't been created yet, so we'll need to create
				// new nodes
				if (currentNode.Children[childIndex] == null) {

					// If the current "zoom level" is == the level of a
					// leaf node, then the new node will be a leaf
					if (zoomLevel == LeafZoomLevel)
						currentNode.Children[childIndex] = new QuadTreeLeafNode();
					else
						currentNode.Children[childIndex] = new QuadTreeNode();
				}

				currentNode = currentNode.Children[childIndex];
				x <<= 1;
				y <<= 1;
				zoomLevel -= 1;
			}

			return (QuadTreeLeafNode)currentNode;
		}

		/// <remarks>
		/// If <paramref name="x"/> >= 0, then 
		/// (<paramref name="x"/> & <see cref="SignalBit"/>) >> <see cref="XRightShift"/> 
		/// ==
		/// 0b00000000_00000000_00000000_00000000
		/// else
		/// 0b00000000_00000000_00000000_00000001
		/// 
		/// If <paramref name="y"/> >= 0, then
		/// (<paramref name="y"/> & <see cref="SignalBit"/>) >> <see cref="YRightShift"/>
		/// ==
		/// 0b00000000_00000000_00000000_00000000
		/// else
		/// 0b00000000_00000000_00000000_00000010
		/// 
		/// Therefore, if <paramref name="x"/> >=   0 and <paramref name="y"/> >=   0, return 0
		/// else,      if <paramref name="x"/> ==   0 and <paramref name="y"/> &lt  0, return 3
		/// else,	   if <paramref name="x"/> &lt  0 and <paramref name="y"/> >=   0, return 1
		/// else	      <paramref name="x"/> &lt  0 and <paramref name="y"/> &lt  0, return 2
		/// </remarks>
		public static int ComputeChildrenIndex(int x, int y) {
			return
				((x & SignalBit) >> XRightShift)
				|
				((y & SignalBit) >> YRightShift);
		}
	}
}
