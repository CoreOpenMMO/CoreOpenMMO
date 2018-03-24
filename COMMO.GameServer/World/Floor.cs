namespace COMMO.GameServer.World {

	public sealed class Floor {
		public const int FloorDiameter = 8;

		/// <summary>
		/// Warning: <see cref="Matrix{T}"/> are mutable and their initial values are null.
		/// </summary>
		public readonly Matrix<Tile> Tiles = new Matrix<Tile>(width: FloorDiameter, height: FloorDiameter);
	}
}