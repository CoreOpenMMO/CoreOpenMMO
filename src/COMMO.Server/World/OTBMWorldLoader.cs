namespace COMMO.Server.World {
	using COMMO.OTB;
	using System;

	/// <summary>
	/// This class contains the methods necessary to load a .otbm file.
	/// </summary>
	public static partial class OTBMWorldLoader {

		/// <summary>
		/// This class only support items encoded using this major version.
		/// </summary>
		public const uint SupportedItemEncodingMajorVersion = 3;

		/// <summary>
		/// This class only support items encoded using this minor version.
		/// </summary>
		public const uint SupportedItemEncodingMinorVersion = 8;

		/// <summary>
		/// Loads a .otbm file, parse it's contents and returns a <see cref="COMMO.Server.World.World"/>.
		/// </summary>
		public static World LoadWorld(ReadOnlyMemory<byte> serializedWorldData) {

			var world = new World();
			var otbTree = OTBDeserializer.DeserializeOTBData(serializedWorldData);

			ParseOTBTreeRootNode(otbTree);

			throw new NotImplementedException();
		}

		/// <summary>
		/// Logs the information embedded in the root node of the OTB tree.
		/// </summary>
		private static void ParseOTBTreeRootNode(OTBNode rootNode) {
			if (rootNode == null)
				throw new ArgumentNullException(nameof(rootNode));
			if (rootNode.Children.Count != 1)
				throw new InvalidOperationException();

			var parsingStream = new OTBParsingStream(rootNode.Data);

			var headerVersion = parsingStream.ReadUInt32();
			if (headerVersion == 0 || headerVersion > 2)
				throw new InvalidOperationException();

			var worldWidth = parsingStream.ReadUInt16();
			var worldHeight = parsingStream.ReadUInt16();

			var itemEncodingMajorVersion = parsingStream.ReadUInt32();
			if (itemEncodingMajorVersion != SupportedItemEncodingMajorVersion)
				throw new InvalidOperationException();

			var itemEncodingMinorVersion = parsingStream.ReadUInt32();
			if (itemEncodingMinorVersion < SupportedItemEncodingMinorVersion)
				throw new InvalidOperationException();

			// TODO: use decent loggin methods
			Console.WriteLine($"OTBM header version: {headerVersion}");
			Console.WriteLine($"World width: {parsingStream.ReadUInt16()}");
			Console.WriteLine($"World height: {parsingStream.ReadUInt16()}");
			Console.WriteLine($"Item encoding major version: {parsingStream.ReadUInt32()}");
			Console.WriteLine($"Item encoding minor version: {parsingStream.ReadUInt32()}");
		}
	}
}
