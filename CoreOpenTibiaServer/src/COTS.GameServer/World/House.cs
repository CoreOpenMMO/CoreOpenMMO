using System;
using System.Collections.Generic;

namespace COTS.GameServer.World {

    public sealed class House {
        public readonly UInt32 Id;
        public string HouseName;
        public Position EntryPosition;

        public readonly List<HouseTile> Tiles = new List<HouseTile>();

        public House(UInt32 id) {
            this.Id = id;
        }

        public void AddTile(HouseTile tile) {
            if (tile == null)
                throw new ArgumentNullException(nameof(tile));

            Tiles.Add(tile);
        }
    }
}