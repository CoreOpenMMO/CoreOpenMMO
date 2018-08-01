using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;
using COMMO.Server.Map;
using System;

namespace COMMO.Server.World {
	public sealed class LazyWorldWrapper : IMapLoader {
		private World _world;
		private Memory<byte> _serializedWorldData;

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
		
		public ITile GetTile(Location location) 
		{
			if (_world == null) {
				_world = OTBMWorldLoader.LoadWorld(_serializedWorldData);
				_serializedWorldData = null;
			}

			return  _world.GetTile(location);
		}

		public LazyWorldWrapper(Memory<byte> serializedWorldData) {
			_serializedWorldData = serializedWorldData;
		}
	}
}
