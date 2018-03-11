using System;
using System.Collections.Generic;

namespace COMMO.GameServer.OTBParsing {

	public sealed class OTBTreeBuilder {
		private readonly byte[] _serializedTreeData;
		private readonly Stack<int> _nodeStarts = new Stack<int>();
		private readonly Stack<int> _childrenCounts = new Stack<int>();
		private readonly Stack<OTBNode> _builtNodes = new Stack<OTBNode>();

		public OTBTreeBuilder(byte[] serializedTreeData) {
			if (serializedTreeData == null)
				throw new ArgumentNullException(nameof(serializedTreeData));

			_serializedTreeData = serializedTreeData;
		}

		public void AddNodeStart(int start) {
			if (start < 0 || start > _serializedTreeData.Length)
				throw new ArgumentOutOfRangeException(nameof(start));

			// Sanity check
			if (_nodeStarts.TryPeek(out var lastStart)) {
				if (start <= lastStart)
					throw new InvalidOperationException();
			}

			_nodeStarts.Push(start);
			_childrenCounts.Push(0);
		}

		public void AddNodeEnd(int end) {
			if (end < 0 || end > _serializedTreeData.Length)
				throw new ArgumentOutOfRangeException();

			// Sanity check
			if (_nodeStarts.Count == 0)
				throw new InvalidOperationException();

			// Marking node's data
			var start = _nodeStarts.Pop();
			var data = new Memory<byte>(
				array: _serializedTreeData,
				start: start,
				length: end - start);

			// Checking if this node has children
			var childCount = _childrenCounts.Pop();
			var currentNodeChildren = new OTBNode[childCount];
			for (int i = 0; i < childCount; i++)
				currentNodeChildren[i] = _builtNodes.Pop();

			// Updating parent's child count
			if (_childrenCounts.Count > 0)
				_childrenCounts.Push(_childrenCounts.Pop() + 1);

			// Creating node and storing it
			var node = new OTBNode(
				children: ReadOnlyArray<OTBNode>.WrapCollection(currentNodeChildren),
				data: data);
			_builtNodes.Push(node);
		}

		public OTBNode BuildTree() {
			if (_builtNodes.Count != 1)
				throw new InvalidOperationException();
			if (_nodeStarts.Count != 0)
				throw new InvalidOperationException();
			if (_childrenCounts.Count != 0)
				throw new InvalidOperationException();

			return _builtNodes.Pop();
		}
	}
}