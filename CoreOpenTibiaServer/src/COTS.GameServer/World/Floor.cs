namespace COTS.GameServer.World {

    public sealed class Floor {

        /// <summary>
        /// This value came from TFS's Map implementation.
        /// As far as I know, it's 8 because that's the size of a player's
        /// `view window'.
        /// </summary>
        public const ushort FloorSize = 8;

        /// <summary>
        /// Warning: <see cref="Matrix{T}"/> are mutable and it's initial values are null.
        /// </summary>
        public readonly Matrix<Tile> Tiles = new Matrix<Tile>(FloorSize, FloorSize);
    }
}