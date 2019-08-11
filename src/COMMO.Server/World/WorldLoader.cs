using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;
using COMMO.Server.Map;
using System;

namespace COMMO.Server.World {
	public sealed class WorldLoader : IMapLoader {
		private readonly World _world;

		public byte PercentageComplete {
			get {
				if (_world == null)
					return 0;
				else
					return _world.PercentageComplete;
			}
		}

		public bool HasLoaded(int x, int y, byte z) {
			if (_world == null)
				return false;
			else
				return _world.HasLoaded(x, y, z);
		}
		
		public ITile GetTile(Location location) => _world.GetTile(location);

		public WorldLoader(string worldFile)
		{ 
			_world = OTBMWorldLoader.LoadWorld(worldFile); 
			Console.WriteLine($"Tiles loaded in world: {_world.LoadedTilesCount()}");
		}
	}
}
