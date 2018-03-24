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
		public readonly Coordinate Position;
		public readonly bool BelongsToHouse;

		public readonly List<Item> Items = new List<Item>();
		public readonly List<PlayerCharacter> PlayerCharacters = new List<PlayerCharacter>();

		public readonly TileFlags Flags;

		public Tile(Coordinate position, TileFlags flags, bool belongsToHouse, List<Item> items) {
			if (items == null)
				throw new ArgumentNullException(nameof(items));

			Position = position;
			Flags = flags;
			BelongsToHouse = belongsToHouse;
			Items = items;
		}
	}
}