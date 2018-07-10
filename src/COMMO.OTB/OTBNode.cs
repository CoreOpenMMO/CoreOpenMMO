using System;

namespace COMMO.OTB {

	public sealed class OTBNode {
		public readonly OTBNodeType Type;
		public readonly ReadOnlyMemory<OTBNode> Children;
		public readonly ReadOnlyMemory<byte> Data;

		public OTBNode(OTBNodeType type, ReadOnlyMemory<OTBNode> children, ReadOnlyMemory<byte> data) {
			Type = type;
			Children = children;
			Data = data;
		}
	}
}
