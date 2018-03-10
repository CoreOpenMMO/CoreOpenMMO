using System;
using System.Collections.Generic;
using COMMO.GameServer.Items;

namespace COMMO.GameServer.World {

	/// <summary>
	/// This class represents a tile in the world.
	/// It's position is described using 2 ushort and a byte because
	/// the it's a relative position within a "floor"
	/// </summary>
    public sealed class Tile {
		public readonly Position Position;
		public readonly bool BelongsToHouse;

        public readonly List<Item> Items = new List<Item>();
        public readonly List<PlayerCharacter> PlayerCharacters = new List<PlayerCharacter>();

        public TileFlags Flags;

        public Tile(Position position, bool belongsToHouse) {
			Position = position;
			BelongsToHouse = belongsToHouse;
        }

        public void AddInternalThing(Item item) {
            throw new NotImplementedException();
        }

        public void AddThing(object p) {
            throw new NotImplementedException();
        }

        public object GetGround() {
            throw new NotImplementedException();
        }
    }
}