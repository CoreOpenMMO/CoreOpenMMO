using System;

namespace COMMO.OTB {

	/// <summary>
	/// Represents a node in the tree-like .otb format.
	/// </summary>
	/// <remarks>
	/// Objets of this class are used to parse .otb files.
	/// </remarks>
	public sealed class OTBNode {
		/// <summary>
		/// The type of the node.
		/// </summary>
		public readonly OTBNodeType Type;

		/// <summary>
		/// The children of this node.
		/// </summary>
		public readonly ReadOnlyMemory<OTBNode> Children;

		/// <summary>
		/// The data of this node.
		/// </summary>
		public readonly ReadOnlyMemory<byte> Data;

		/// <summary>
		/// Creates a new instance of a <see cref="OTBNode"/>.
		/// </summary>
		public OTBNode(OTBNodeType type, ReadOnlyMemory<OTBNode> children, ReadOnlyMemory<byte> data) {
			Type = type;
			Children = children;
			Data = data;
		}
	}
}
