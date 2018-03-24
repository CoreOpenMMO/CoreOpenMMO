using System;

namespace COMMO.GameServer {

	/// <summary>
	/// Represents a 3D point with integer coordinates.
	/// </summary>
	/// <remarks>
	/// It's important to note that our coordinate system is "cartesian-like".
	/// That means that our origin is at (X=0, Y=0, Z=0).
	/// 
	/// A positive <see cref="X"/> means that the coordinate is to the west of the origin, while
	/// a negative <see cref="X"/> that the coordinate is to the east of the origin.
	/// 
	/// A positive <see cref="Y"/> means that the coordinate is to the north of the origin, while
	/// a negative <see cref="X"/> means that the coordinate is to the south of the origin.
	/// 
	/// A zero <see cref="Z"/> means that the coordinate is on the ground fllor, 
	/// while a positive <see cref="Z"/> means that the coordinate is above 
	/// the ground floor and a negative <see cref="Z"/> means that the coordinate
	/// is underground.
	/// 
	/// This is different from TFS / OTX, where the X and Y values are positive only, with
	/// the origin being on the top-left corner of the map.
	/// Also, the Z coordinate of TFS / OTX must be [0, 15], with 7 representing the ground
	/// floor.
	/// Be careful when converting between the two systems (e.g.: when parsing a .otbm map).
	/// </remarks>
	public readonly struct Coordinate : IEquatable<Coordinate> {
		private const byte TFSGroundFloorZCoordinate = 7;

		/// <summary>
		/// The x coordinate of this instance.
		/// </summary>
		public readonly int X;

		/// <summary>
		/// The y coordinate of this instance.
		/// </summary>
		public readonly int Y;

		/// <summary>
		/// The z coordinate of this instance, use the represent the "floor height".
		/// It's important to note that we use a different representation than TFS / OTX:
		/// In our representation:
		public readonly sbyte Z;

		/// <summary>
		/// Creates a new instance of <see cref="Coordinate"/>.
		/// </summary>
		public Coordinate(int x, int y, sbyte z) {
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>
		/// Creates a <see cref="Coordinate"/> from the coordinates of a 
		/// "TFS Location". See the remarks of <see cref="Coordinate"/> to know more
		/// about the differences between the coordinate systems.
		/// </summary>
		public static Coordinate FromTFSCoordinates(UInt16 x, UInt16 y, byte z) {
			return new Coordinate(
				x: x,
				y: -y,
				z: (sbyte)-(z - TFSGroundFloorZCoordinate));
		}

		/// <summary>
		/// Creates a new <see cref="Coordinate"/> whose coordinates are equal to this instance's
		/// plus the provided offesets.
		/// </summary>
		public Coordinate Translate(int xOffset, int yOffset) => Translate(xOffset: xOffset, yOffset: yOffset, zOffset: Z);

		/// <summary>
		/// Creates a new <see cref="Coordinate"/> whose coordinates are equal to this instance's
		/// plus the provided offesets.
		/// </summary>
		public Coordinate Translate(int xOffset, int yOffset, sbyte zOffset) {
			return new Coordinate(
				x: X + xOffset,
				y: Y + yOffset,
				z: (sbyte)(Z + zOffset));
		}

		/// <summary>
		/// Returns true if <paramref name="obj"/> is a Poisition and it's coordinates are
		/// equal to this one's.
		/// </summary>
		public override bool Equals(object obj) => (obj is Coordinate) && Equals((Coordinate)obj);

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
		/// Returns a string representation of this instance.
		/// </summary>
		public override string ToString() => $"{{X={X}, Y={Y}, Z={Z}}}";

		/// <summary>
		/// Returns true if the other <see cref="Coordinate"/> has the same <see cref="X"/>,
		/// <see cref="Y"/> and <see cref="Z"/> values as this one.
		/// Returns false otherwise.
		/// </summary>
		public bool Equals(Coordinate other) => X == other.X && Y == other.Y && Z == other.Z;

		/// <summary>
		/// Compares two <see cref="Coordinate"/> objects. The result specifies whether the values
		/// of the <see cref="X"/>, <see cref="Y"/> and <see cref="Z"/> fields of the two
		/// <see cref="Coordinate"/> objects are equal.
		/// </summary>
		public static bool operator ==(Coordinate first, Coordinate second) => first.Equals(second);

		/// <summary>
		/// Compares two <see cref="Coordinate"/> objects. The result specifies whether the values
		/// of the <see cref="X"/> or <see cref="Y"/> or <see cref="Z"/> fields of the two
		/// <see cref="Coordinate"/> objects are unequal.
		/// </summary>
		public static bool operator !=(Coordinate first, Coordinate second) => !(first == second);
	}
}