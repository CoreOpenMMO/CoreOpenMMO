using System;

namespace COMMO.GameServer.OTBParsing {

	public sealed class OTBNode {
		public readonly OTBNodeType Type;
		public readonly ReadOnlyArray<OTBNode> Children;
		public readonly ReadOnlyMemory<byte> Data;

		public OTBNode(OTBNodeType type, ReadOnlyArray<OTBNode> children, ReadOnlyMemory<byte> data) {
			if (children == null)
				throw new ArgumentNullException(nameof(children));

			Type = type;
			Children = children;
			Data = data;
		}
	}
}
