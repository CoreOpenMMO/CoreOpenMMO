using System;

namespace COMMO.GameServer.World {
	public sealed class Town : IEquatable<Town> {
		public readonly uint TownId;
		public readonly string TownName;
		public readonly Coordinate TemplePosition;

		public Town(uint townId, string townName, Coordinate templePosition) {
			if (townName == null)
				throw new ArgumentNullException(nameof(townName));

			TownId = townId;
			TownName = townName;
			TemplePosition = templePosition;
		}

		public override bool Equals(object obj) => Equals(obj as Town);

		public bool Equals(Town other) {
			return other != null &&
				TownId == other.TownId &&
				TownName == other.TownName &&
				TemplePosition.Equals(other.TemplePosition);
		}

		public override int GetHashCode() {
			return HashHelper.Start
				.CombineHashCode(TownId)
				.CombineHashCode(TownName);
		}
	}
}
