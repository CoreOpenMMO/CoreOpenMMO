using System;
using System.Collections.Generic;

namespace COTS.GameServer.World {

    public class Tile {
        public readonly ushort X;
        public readonly ushort Y;
        public readonly byte Z;

        public readonly List<Item> Items = new List<Item>();
        public readonly List<PlayerCharacter> PlayerCharacters = new List<PlayerCharacter>();

        public TileFlags Flags;

        public Tile(ushort x, ushort y, byte z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public void AddInternalThing(Item item) {
            throw new NotImplementedException();
        }

        public void AddThing(object p) {
            throw new NotImplementedException();
        }

        public object GetGround() {
            throw new NotImplementedException();
        }
    }
}