namespace COMMO.Server.Map {
	using COMMO.Server.Data.Interfaces;
	using System;

	public sealed class OTBMWorldLoader : IMapLoader {
		public byte PercentageComplete => throw new NotImplementedException();

		public bool HasLoaded(int x, int y, byte z) {
			throw new NotImplementedException();
		}

		public ITile[,,] Load(int fromSectorX, int toSectorX, int fromSectorY, int toSectorY, byte fromSectorZ, byte toSectorZ) {
			throw new NotImplementedException();
		}
	}
}
