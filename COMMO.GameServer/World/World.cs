using System;

namespace COMMO.GameServer.World {

	public sealed class World {
		/// <summary>
		/// Since we use a 8 bits to represent the z value of a <see cref="Coordinate"/>,
		/// our World can have 256 floors.
		/// </summary>
		public const int FloorCount = 256;

		private readonly QuadTree _quadTree;

		public void AddTile(Tile tile) {
			if (tile == null)
				throw new ArgumentNullException(nameof(tile));

			throw new NotImplementedException();
			//var leafNode = RootQuadTreeNode.CreateLeafOrGetReference(x, y, WorldHighestLayer);

			//UpdateNeighbors(x, y, leafNode);

			//// Updating floor data
			//var floor = leafNode.CreateFloorOrGetReference(z);

			//ushort xOffset = (ushort)(x & Floor.FloorMask);
			//ushort yOffset = (ushort)(y & Floor.FloorMask);

			//// If there is not tile in the given coordinates,
			//// just set it to be the parameter of this method
			//var oldTile = floor.Tiles.Get(xOffset, yOffset);
			//if (oldTile == null) {
			//    floor.Tiles.Set(x, y, newTile);
			//    return;
			//}

			//// If there is already a tile in the given coordinates,
			//// we'll update it's items
			//var newItems = newTile.Items;
			//var newItemCount = newTile.Items.Count;
			//for (int i = 0; i < newItemCount; i++) {
			//    oldTile.AddThing(newItems[i]);
			//}

			//// No idea what a "item.GetGround" is supposed to be
			//if (newTile.GetGround() != null)
			//    oldTile.AddThing(newTile.GetGround());
		}

		public bool TryGetTile(in Coordinate coordinate, out Tile tile) {
			if (!_quadTree.TryGetLeaf(coordinate.X, coordinate.Y, out var leaf)) {
				tile = null;
				return false;
			}
			if (!leaf.TryGetFloor(coordinate.Z, out var floor)) {
				tile = null;
				return false;
			}

			tile = floor.Tiles.Get(coordinate.X, coordinate.Y);
			return tile != null;
		}
	}
}