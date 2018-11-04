namespace COMMO.OTB.Tests.OTBMTestWorlds {
	using NUnit.Framework;
	using System;
	using System.IO;

	public static class OTBMTestWorldsHelper {
		public enum Worlds { Empty, DirtAndMexcalibur, AutoPlacedGrass }

		public static Memory<byte> GetSerializedWorldData(Worlds world) {
			var testWorldsDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory,
			"..", "..", "..", "..", "OTBMTestWorlds");

			switch (world) {
				case Worlds.Empty:
				return File.ReadAllBytes(Path.Combine(testWorldsDirectory, "Empty.otbm"));

				case Worlds.DirtAndMexcalibur:
				return File.ReadAllBytes(Path.Combine(testWorldsDirectory, "DirtAndMexcalibur.otbm"));

				case Worlds.AutoPlacedGrass:
				return File.ReadAllBytes(Path.Combine(testWorldsDirectory, "AutoPlacedGrass.otbm"));
					

				default:
				throw new InvalidOperationException();
			}
		}
	}
}
