namespace COMMO.Server.World {
	using System;
	using System.IO;

	/// <summary>
	/// This class contains the methods necessary to load a .otbm file.
	/// </summary>
	public static partial class OTBMWorldLoader {

		/// <summary>
		/// Loads a .otbm file, parse it's contents and returns a <see cref="COMMO.Server.World.World"/>.
		/// </summary>
		public static World LoadWorld(string filename) {
			if (filename == null)
				throw new ArgumentNullException(nameof(filename));

			var serializedWorldData = File.ReadAllBytes(filename);

			throw new NotImplementedException();
		}
	}
}
