namespace COMMO.Server {
	using COMMO.Server.Data.Interfaces;
	using System.Collections.Generic;

	public class CreatureEqualityComparer : IEqualityComparer<ICreature> {
		public bool Equals(ICreature x, ICreature y) {
			return x.CreatureId == y.CreatureId;
		}

		public int GetHashCode(ICreature obj) {
			return (int)obj.CreatureId;
		}
	}
}
