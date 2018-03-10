using System;

namespace COMMO.GameServer.OTBParsing {

	public sealed class OTBNode {
		public readonly ReadOnlyArray<OTBNode> Children;
		public readonly ReadOnlyMemory<byte> Data;

		public OTBNode(ReadOnlyArray<OTBNode> children, ReadOnlyMemory<byte> data) {
			if (children == null)
				throw new ArgumentNullException(nameof(children));

			Children = children;
			Data = data;
		}
	}
}
