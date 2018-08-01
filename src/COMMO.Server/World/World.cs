namespace COMMO.Server.World {
	using COMMO.Server.Data.Interfaces;
	using COMMO.Server.Data.Models.Structs;
	using COMMO.Server.Map;
	using System;
	using System.Collections.Concurrent;
	using System.Linq;

	/// <summary>
	/// This class is meant to be a in-memory substitute for <see cref="SectorMapLoader"/>.
	/// The current implementation of <see cref="World"/> is slow and memory hungry, but it's meant
	/// to be just test the world loading functionality.
	/// We will refactor and improve it later.
	/// </summary>
	public sealed class World : IMapLoader {

		public byte PercentageComplete => 100;
		public bool HasLoaded(int x, int y, byte z) => _worldTiles.Any();
		public int LoadedTilesCount() => _worldTiles.Count();

		private readonly ConcurrentDictionary<Coordinate, Tile> _worldTiles = new ConcurrentDictionary<Coordinate, Tile>();
		
		public void AddTile(Tile tile) {
			if (tile == null)
				throw new ArgumentNullException(nameof(tile));

			var tilesCoordinates = new Coordinate(
				x: tile.Location.X,
				y: tile.Location.Y,
				z: tile.Location.Z);

			_worldTiles[tilesCoordinates] = tile;
		}

		public ITile GetTile(Location location)
		{
			if(_worldTiles.TryGetValue(new Coordinate(location.X, location.Y, location.Z), out Tile tile))
				return tile;

			return null;
		} 

	}
}
