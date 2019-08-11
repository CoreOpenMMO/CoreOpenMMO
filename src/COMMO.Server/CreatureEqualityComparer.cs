namespace COMMO.Server {
	using COMMO.Server.Data.Interfaces;
	using System.Collections.Generic;

	public class CreatureEqualityComparer : IEqualityComparer<ICreature> {
		public bool Equals(ICreature x, ICreature y) => x.CreatureId == y.CreatureId;

		public int GetHashCode(ICreature obj) => (int) obj.CreatureId;
	}
}
