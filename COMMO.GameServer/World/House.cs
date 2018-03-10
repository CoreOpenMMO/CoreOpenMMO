using System;
using System.Collections.Generic;

namespace COMMO.GameServer.World {

	public sealed class House {
		public readonly UInt32 Id;
		public string HouseName;
		public Position EntryPosition;

		public readonly List<Tile> Tiles = new List<Tile>();

		public int LeftmostTileCoordinate { get; private set; }
		public int TopmostTileCoordinate { get; private set; }
		public int RightmostTileCoordinate { get; private set; }
		public int BottommostTileCoordinate { get; private set; }

		public House(UInt32 id) {
			Id = id;
		}

		public void AddTile(Tile tile) {
			if (tile == null)
				throw new ArgumentNullException(nameof(tile));

			Tiles.Add(tile);

			LeftmostTileCoordinate = Math.Min(LeftmostTileCoordinate, tile.Position.X);
			TopmostTileCoordinate = Math.Min(TopmostTileCoordinate, tile.Position.Y);
			RightmostTileCoordinate = Math.Max(RightmostTileCoordinate, tile.Position.X);
			BottommostTileCoordinate = Math.Max(BottommostTileCoordinate, tile.Position.Y);
		}

		public bool TryGetTile(ushort tileXCoordinate, ushort tileYCoordinate, byte tileZCoordinate, out Tile tile) {
			if (tileXCoordinate < LeftmostTileCoordinate ||
				tileXCoordinate > RightmostTileCoordinate ||
				tileYCoordinate < TopmostTileCoordinate ||
				tileYCoordinate > BottommostTileCoordinate) {

				tile = null;
				return false;
			}

			for (int i = 0; i < Tiles.Count; i++) {
				var currentTile = Tiles[i];
				if (currentTile.Position.X == tileXCoordinate &&
					currentTile.Position.Y == tileYCoordinate &&
					currentTile.Position.Z == tileZCoordinate) {

					tile = currentTile;
					return true;
				}
			}

			tile = null;
			return false;
		}
	}
}