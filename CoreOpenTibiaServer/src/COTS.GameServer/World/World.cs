using System;

namespace COTS.GameServer.World {

    public sealed class World {
        public const byte WorldHighestLayer = 15;

        public readonly QuadTreeNode RootQuadTreeNode;

        public bool TryGetTile(ushort x, ushort y, byte z, out Tile tile) {
            if (z > WorldHighestLayer)
                throw new ArgumentOutOfRangeException(nameof(z) + $" must be equal to or less than {WorldHighestLayer}");

            if (!RootQuadTreeNode.TryGetLeaf(x, y, out var leaf)) {
                tile = null;
                return false;
            }
            if (!leaf.TryGetFloor(z, out var floor)) {
                tile = null;
                return false;
            }

            tile = floor.Tiles.Get(x, y);
            return tile != null;
        }

        public void CreateOrUpdateTile(ushort x, ushort y, byte z, Tile newTile) {
            if (z > WorldHighestLayer)
                throw new ArgumentOutOfRangeException(nameof(z) + $" must be equal to or less than {WorldHighestLayer}");
            if (newTile == null)
                throw new ArgumentNullException(nameof(newTile));

            var leafNode = RootQuadTreeNode.CreateLeafOrGetReference(x, y, WorldHighestLayer);
            
            UpdateNeighbors(x, y, leafNode);

            // Updating floor data
            var floor = leafNode.CreateFloorOrGetReference(z);

            ushort xOffset = (ushort)(x & Floor.FloorMask);
            ushort yOffset = (ushort)(y & Floor.FloorMask);

            // If there is not tile in the given coordinates,
            // just set it to be the parameter of this method
            var oldTile = floor.Tiles.Get(xOffset, yOffset);
            if (oldTile == null) {
                floor.Tiles.Set(x, y, newTile);
                return;
            }

            // If there is already a tile in the given coordinates,
            // we'll update it's items
            var newItems = newTile.Items;
            var newItemCount = newTile.Items.Count;
            for (int i = 0; i < newItemCount; i++) {
                oldTile.AddThing(newItems[i]);
            }

            // No idea what a "item.GetGround" is supposed to be
            if (newTile.GetGround() != null)
                oldTile.AddThing(newTile.GetGround());
        }

        private void UpdateNeighbors(ushort leafsXCoordinate, ushort leafsYCoordinate, QuadTreeLeafNode leafNode) {
            if (RootQuadTreeNode.TryGetLeaf(
                            x: leafsXCoordinate,
                            y: (ushort)(leafsYCoordinate - Floor.FloorSize),
                            leaf: out var northLeaf)) {
                northLeaf.SouthNeighbor = leafNode;
            }

            if (RootQuadTreeNode.TryGetLeaf(
                x: (ushort)(leafsXCoordinate - Floor.FloorSize),
                y: leafsYCoordinate,
                leaf: out var westLeaf)) {
            }

            if (RootQuadTreeNode.TryGetLeaf(
                x: leafsXCoordinate,
                y: (ushort)(leafsYCoordinate + Floor.FloorSize),
                leaf: out var southLeaf)) {
                leafNode.SouthNeighbor = southLeaf;
            }

            if (RootQuadTreeNode.TryGetLeaf(
                x: (ushort)(leafsXCoordinate + Floor.FloorSize),
                y: leafsYCoordinate,
                leaf: out var eastLeaf)) {
                leafNode.EastNeighbor = eastLeaf;
            }
        }
    }
}