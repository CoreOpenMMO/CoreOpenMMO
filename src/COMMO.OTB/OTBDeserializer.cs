namespace COMMO.OTB {
	using COMMO.Utilities;
	using System;

	/// <summary>
	/// This class provides simple methods to convert a .otb serialized data
	/// back to a OTB tree.
	/// </summary>
	public static class OTBDeserializer {

		/// <summary>
		/// Parses data serialized as .otb and returns the deserialized .otb tree structure.
		/// </summary>
		/// <remarks>
		/// Beware that some .otb serializers add a "format identifier" before the data.
		/// Maps (worlds?), for instance, contain 4 "format identifier bytes" that should be skiped.
		/// </remarks>
		public static OTBNode DeserializeOTBData(ReadOnlyMemory<byte> serializedOTBData) {
			var stream = new ReadOnlyMemoryStream(serializedOTBData);

			// Skipping the first 4 bytes coz they are used to store a... identifier?
			// stream.Skip(IdentifierLength);

			var treeBuilder = new OTBTreeBuilder(serializedOTBData);
			while (!stream.IsOver) {
				var currentMark = (OTBMarkupByte)stream.ReadByte();
				if (currentMark < OTBMarkupByte.Escape) {
					// Since <see cref="OTBMarkupByte"/> can only have values Escape (0xFD), Start (0xFE) and
					// End (0xFF), if currentMark < Escape, then it's just prop data 
					// and we can safely skip it.
					continue;
				}

				switch (currentMark) {
					case OTBMarkupByte.Start:
					var nodeType = (OTBNodeType)stream.ReadByte();
					treeBuilder.AddNodeDataBegin(
						start: stream.Position,
						type: nodeType);
					break;

					case OTBMarkupByte.End:
					treeBuilder.AddNodeEnd(stream.Position);
					break;

					case OTBMarkupByte.Escape:
					stream.Skip();
					break;

					default:
					throw new InvalidOperationException();
				}
			}

			return treeBuilder.BuildTree();
		}
	}
}
