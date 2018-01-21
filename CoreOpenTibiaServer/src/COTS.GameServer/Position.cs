using System;

namespace COTS.GameServer {

    /// <summary>
    /// Represents a 3D point with integer coordinates.
    /// </summary>
    public  struct Position : IEquatable<Position> {

        /// <summary>
        /// The x coordinate of this instance.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// The y coordinate of this instance.
        /// </summary>
        public readonly int Y;

        /// <summary>
        /// The z coordinate of this instance.
        /// </summary>
        public readonly int Z;

        /// <summary>
        /// Creates a new instance of <see cref="Position"/>.
        /// </summary>
        public Position(int x, int y, int z) {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Returns true if <paramref name="obj"/> is a Poisition and it's coordinates are
        /// equal to this one's.
        /// </summary>
        public override bool Equals(object obj) => (obj is Position) && Equals((Position)obj);

        /// <summary>
        /// Returns the hash code of this instance.
        /// </summary>
        public override int GetHashCode() {
            return HashHelper.Start
                .CombineHashCode(X)
                .CombineHashCode(Y)
                .CombineHashCode(Z);
        }

        /// <summary>
        /// Returns a string with the coordinates of this instance.
        /// </summary>
        public override string ToString() => $"{{X={X}, Y={Y}, Z={Z}}}";

        /// <summary>
        /// Returns true if the other Position has the same coordinates as this one.
        /// </summary>
        public bool Equals(Position other) => X == other.X && Y == other.Y && Z == other.Z;

        /// <summary>
        /// Returns true if both Positions have the same coordinates.
        /// </summary>
        public static bool operator ==(Position first, Position second) => first.Equals(second);

        /// <summary>
        /// Returns true if the Positions have at least one different coordinate.
        /// </summary>
        public static bool operator !=(Position first, Position second) => !(first == second);
    }
}