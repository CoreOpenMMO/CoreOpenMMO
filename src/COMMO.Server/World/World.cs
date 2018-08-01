namespace COMMO.Server.World {
	using COMMO.Server.Data.Interfaces;
	using COMMO.Server.Map;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// This class is meant to be a in-memory substitute for <see cref="SectorMapLoader"/>.
	/// The current implementation of <see cref="World"/> is slow and memory hungry, but it's meant
	/// to be just test the world loading functionality.
	/// We will refactor and improve it later.
	/// </summary>
	public sealed class World : IMapLoader {
		public const int SectorXLength = 32;
		public const int SectorYLength = 32;
		public const int SectorZLength = 1;

		public byte PercentageComplete => 100;
		public bool HasLoaded(int x, int y, byte z) => _loadedTiles.Any(c => c.X == x && c.Y == y && c.Z == z);

		private List<LoadedTile> _loadedTiles;

		private readonly Dictionary<Coordinate, Tile> _worldTiles = new Dictionary<Coordinate, Tile>();
		
		public ITile[,,] Load(int fromSectorX, int toSectorX, int fromSectorY, int toSectorY, byte fromSectorZ, byte toSectorZ) {
			if (toSectorX < fromSectorX)
				throw new ArgumentOutOfRangeException();
			if (toSectorY < fromSectorY)
				throw new ArgumentOutOfRangeException();
			if (toSectorZ < fromSectorZ)
				throw new ArgumentOutOfRangeException();
			
			if(_loadedTiles == null)
				_loadedTiles = new List<LoadedTile>();

			var tiles = new ITile[
				(toSectorX - fromSectorX + 1) * SectorXLength,
				(toSectorY - fromSectorY + 1) * SectorYLength,
				toSectorZ - fromSectorZ + SectorZLength
				];
			
			var xStart = fromSectorX * SectorXLength;
            var xEnd = ((toSectorX - fromSectorX + 1) * SectorXLength) + xStart;

            var yStart = fromSectorY * SectorYLength;
            var yEnd = ((toSectorY - fromSectorY + 1) * SectorYLength) + yStart;

            var zStart = (sbyte)(fromSectorZ * SectorZLength);
            var zEnd = (sbyte)(((toSectorZ- fromSectorZ + 1) * SectorZLength) + zStart);

			for (var x = xStart; x < xEnd; x++) {
				for (var y = yStart; y < yEnd; y++) {
					for (var z = zStart; z < zEnd; z++) {
						var tileCoordinate = new Coordinate(x: x, y: y, z);
							var tile = _worldTiles.GetValueOrDefault(tileCoordinate) ?? new Tile((ushort)x, (ushort)y, z);
							tiles[x - xStart, y - yStart, z - zStart] = tile;
							_loadedTiles.Add(new LoadedTile { X =  x - xStart, Y = y - yStart, Z = z - zStart} );
					}
				}
			}

			return tiles;
		}

		public void AddTile(Tile tile) {
			if (tile == null)
				throw new ArgumentNullException(nameof(tile));

			var tilesCoordinates = new Coordinate(
				x: tile.Location.X,
				y: tile.Location.Y,
				z: tile.Location.Z);

			_worldTiles[tilesCoordinates] = tile;
		}
	}

	public class LoadedTile
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
	}
}
