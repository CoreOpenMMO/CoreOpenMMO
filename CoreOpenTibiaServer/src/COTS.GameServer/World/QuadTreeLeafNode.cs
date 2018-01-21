using System;
using System.Collections.Generic;

namespace COTS.GameServer.World {

    /// <summary>
    /// This class represents the most "zoomed-in" part of the World,
    /// therefore it doesn't contain children nodes.
    /// </summary>
    public sealed class QuadTreeLeafNode : QuadTreeNode {
        public QuadTreeLeafNode SouthNeighbor;
        public QuadTreeLeafNode EastNeighbor;

        public readonly List<PlayerCharacter> Players = new List<PlayerCharacter>();
        public readonly List<Monster> Monsters = new List<Monster>();

        private readonly Floor[] _floors = new Floor[World.WorldHighestLayer];

        public QuadTreeLeafNode()
            : base(isLeaf: true) {
        }

        // We lazily create floors to save memory.
        // We could eargily create them to increase performance tho
        public Floor CreateFloorOrGetReference(byte z) {
            if (z > World.WorldHighestLayer)
                throw new ArgumentOutOfRangeException(nameof(z) + $" must be equal to or less than {World.WorldHighestLayer}");

            if (_floors[z] == null)
                _floors[z] = new Floor();

            return _floors[z];
        }

        public bool TryGetFloor(byte z, out Floor floor) {
            if (z > World.WorldHighestLayer)
                throw new ArgumentOutOfRangeException(nameof(z) + $" must be equal to or less than {World.WorldHighestLayer}");

            floor = _floors[z];
            return floor != null;
        }
    }
}