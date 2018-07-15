namespace COMMO.Server.World {
	using COMMO.Server.Data.Interfaces;
	using COMMO.Server.Map;

	public sealed class World : IMapLoader {		
		public byte PercentageComplete => 100;
		public bool HasLoaded(int x, int y, byte z) => true;

		public ITile[,,] Load(int fromSectorX, int toSectorX, int fromSectorY, int toSectorY, byte fromSectorZ, byte toSectorZ) {
			throw new System.NotImplementedException();
		}
	}
}
