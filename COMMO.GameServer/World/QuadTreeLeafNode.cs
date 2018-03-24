namespace COMMO.GameServer.World {

	/// <summary>
	/// This class represents the most "zoomed-in" part of the QuadTree,
	/// therefore it contains no children.
	/// </summary>
	/// <remarks>
	/// This class is built on the assumption that <see cref="Coordinate.Z"/>
	/// is a sbyte!
	/// </remarks>
	public sealed class QuadTreeLeafNode : QuadTreeNode {
		private readonly Floor[] _floors = new Floor[World.FloorCount];

		public QuadTreeLeafNode()
			: base(isLeaf: true) {
		}

		public Floor CreateFloorOrGetReference(sbyte z) {
			// This way we "wrap around" for negative values
			var floorIndex = (byte)z;

			if (_floors[floorIndex] == null)
				_floors[floorIndex] = new Floor();

			return _floors[floorIndex];
		}

		public bool TryGetFloor(sbyte z, out Floor floor) {
			// This way we "wrap around" for negative values
			var floorIndex = (byte)z;

			floor = _floors[floorIndex];
			return floor != null;
		}
	}
}