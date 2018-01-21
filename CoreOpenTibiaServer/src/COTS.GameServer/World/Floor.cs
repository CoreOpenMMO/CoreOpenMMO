namespace COTS.GameServer.World {

    public sealed class Floor {
        
        /// <summary>
        /// No clue why this is 3... Just copying the C++ implementation.
        /// </summary>
        public const ushort FloorBits = 3;

        /// <summary>
        /// No clue why this is 1 << FloorBits... Just copying the C++ implementation.
        /// </summary>
        public const ushort FloorSize = 1 << FloorBits;

        /// <summary>
        /// No clue why this is FloorSize -1... Just Copying the C++ implementation
        /// </summary>
        public const ushort FloorMask = FloorSize - 1;

        /// <summary>
        /// Warning: <see cref="Matrix{T}"/> are mutable and their initial values are null.
        /// </summary>
        public readonly Matrix<Tile> Tiles = new Matrix<Tile>(FloorSize, FloorSize);
    }
}