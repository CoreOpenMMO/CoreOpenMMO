using System;

namespace COMMO.GameServer.World {

	public class QuadTreeNode {
		private const int QuadTreeNodeChildCount = 4;
		public readonly QuadTreeNode[] Children;

		public readonly bool IsLeaf;

		protected QuadTreeNode(bool isLeaf) {
			IsLeaf = isLeaf;
			if (IsLeaf)
				Children = Array.Empty<QuadTreeNode>();
			else
				Children = new QuadTreeNode[QuadTreeNodeChildCount];
		}

		public QuadTreeNode()
			: this(isLeaf: false) { }
	}
}