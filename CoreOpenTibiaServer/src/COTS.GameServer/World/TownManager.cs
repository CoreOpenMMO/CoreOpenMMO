using System;
using System.Collections.Generic;
using System.Linq;

namespace COTS.GameServer.World {
	public sealed class TownManager {
		private HashSet<Town> _towns = new HashSet<Town>();

		private TownManager() { }

		public static TownManager Instance = new TownManager();

		public void AddTown(Town town) {
			if (town == null)
				throw new ArgumentNullException(nameof(town));

			if (_towns.Contains(town))
				throw new ArgumentException(nameof(town) + " was added already.");
			else
				_towns.Add(town);
		}

		public Town GetTownById(uint townId) {
			var town = _towns.FirstOrDefault(t => t.TownId == townId);
			return town ?? throw new ArgumentOutOfRangeException($"There is no town with id {townId};");
		}

		public Town GetTownByName(string townName) {
			var town = _towns.FirstOrDefault(t => t.TownName == townName);
			return town ?? throw new ArgumentOutOfRangeException($"There is no town with id {townName};");
		}
	}
}
