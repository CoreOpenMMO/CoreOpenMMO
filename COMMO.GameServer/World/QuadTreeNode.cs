namespace COMMO.GameServer.World {

	public class QuadTreeNode {
		private const int QuadTreeNodeChildCount = 4;
		public readonly QuadTreeNode[] Children = new QuadTreeNode[QuadTreeNodeChildCount];

		public readonly bool IsLeaf;

		protected QuadTreeNode(bool isLeaf) {
			IsLeaf = isLeaf;
		}

		public QuadTreeNode()
			: this(isLeaf: false) { }
	}
}