using System;
using System.Collections.Generic;
using System.Linq;

namespace COMMO.GameServer.World {
	public sealed class TownManager {
		private const int GuessedTownCount = 128;
		private List<Town> _towns = new List<Town>(capacity: GuessedTownCount);

		private TownManager() { }

		public static readonly TownManager Instance = new TownManager();

		public void AddTown(Town town) {
			if (town == null)
				throw new ArgumentNullException(nameof(town));

			for (int i = 0; i < _towns.Count; i++) {
				var currentTown = _towns[i];
				if (currentTown.TownId == town.TownId)
					throw new ArgumentException(nameof(town) + $" town with same {nameof(town.TownId)} already added.");
				if (currentTown.TownName == town.TownName)
					throw new ArgumentException(nameof(town) + $" town with same {nameof(town.TownName)} already added.");
				if (currentTown.TemplePosition == town.TemplePosition)
					throw new ArgumentException(nameof(town) + $" town with same {nameof(town.TemplePosition)} already added.");
			}

			_towns.Add(town);
		}

		public Town GetTownById(uint townId) {
			var town = _towns.FirstOrDefault(t => t.TownId == townId);
			return town ?? throw new ArgumentOutOfRangeException($"There is no town with id {townId};");
		}

		public Town GetTownByName(string townName) {
			if (townName == null)
				throw new ArgumentNullException(nameof(townName));

			var town = _towns.FirstOrDefault(t => t.TownName == townName);
			return town ?? throw new ArgumentOutOfRangeException($"There is no town with id {townName};");
		}
	}
}
