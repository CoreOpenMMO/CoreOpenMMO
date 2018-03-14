using System;
using System.Collections.Generic;

namespace COMMO.GameServer.OTBParsing {

	public sealed class OTBTreeBuilder {
		private readonly byte[] _serializedTreeData;
		private readonly Stack<int> _nodeStarts = new Stack<int>();
		private readonly Stack<OTBNodeType> _nodeTypes = new Stack<OTBNodeType>();
		private readonly Stack<int> _childrenCounts = new Stack<int>();
		private readonly Stack<OTBNode> _builtNodes = new Stack<OTBNode>();

		public OTBTreeBuilder(byte[] serializedTreeData) {
			if (serializedTreeData == null)
				throw new ArgumentNullException(nameof(serializedTreeData));

			_serializedTreeData = serializedTreeData;
		}

		/// <remarks>Start will be included in the data.</remarks>
		public void AddNodeDataBegin(int start, OTBNodeType type) {
			if (start < 0 || start > _serializedTreeData.Length)
				throw new ArgumentOutOfRangeException(nameof(start));

			// Sanity check
			if (_nodeStarts.TryPeek(out var lastStart)) {
				if (start <= lastStart)
					throw new InvalidOperationException();
			}

			_nodeStarts.Push(start);
			_childrenCounts.Push(0);
			_nodeTypes.Push(type);
		}

		/// <remarks>End will not be included in the data.</remarks>
		public void AddNodeEnd(int end) {
			if (end < 0 || end > _serializedTreeData.Length)
				throw new ArgumentOutOfRangeException();

			// Sanity checks
			if (!_nodeStarts.TryPop(out var start))
				throw new InvalidOperationException();
			if (end < start)
				throw new InvalidOperationException();

			var data = new Memory<byte>(
				array: _serializedTreeData,
				start: start,
				length: end - start);

			// Checking if this node has children
			if (!_childrenCounts.TryPop(out var childCount))
				throw new InvalidOperationException();

			var currentNodeChildren = new OTBNode[childCount];
			for (int i = 0; i < childCount; i++)
				currentNodeChildren[i] = _builtNodes.Pop();

			// Updating parent's child count
			if (_childrenCounts.Count > 0)
				_childrenCounts.Push(_childrenCounts.Pop() + 1);

			// Creating node and storing it
			var node = new OTBNode(
				type: _nodeTypes.Pop(),
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