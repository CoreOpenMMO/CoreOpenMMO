using COMMO.Server.Data.Interfaces;
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

		public ITile[,,] Load(int fromSectorX, int toSectorX, int fromSectorY, int toSectorY, byte fromSectorZ, byte toSectorZ) {
			if (_world == null) {
				_world = OTBMWorldLoader.LoadWorld(_serializedWorldData);
				_serializedWorldData = null;
			}

			return _world.Load(
				fromSectorX: fromSectorX,
				toSectorX: toSectorX,
				fromSectorY: fromSectorY,
				toSectorY: toSectorY,
				fromSectorZ: fromSectorZ,
				toSectorZ: toSectorZ);
		}

		public LazyWorldWrapper(Memory<byte> serializedWorldData) {
			_serializedWorldData = serializedWorldData;
		}
	}
}
