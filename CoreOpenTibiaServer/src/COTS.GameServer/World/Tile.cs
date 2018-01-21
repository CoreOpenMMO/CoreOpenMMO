using System;
using System.Collections.Generic;

namespace COTS.GameServer.World {

    public class Tile {
        public readonly ushort X;
        public readonly ushort Y;
        public readonly byte Z;

        public readonly Stack<Item> Items = new Stack<Item>();
        public readonly Stack<PlayerCharacter> PlayerCharacters = new Stack<PlayerCharacter>();

        public TileFlags Flags;

        public Tile(ushort x, ushort y, byte z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        internal void AddInternalThing(Item item) {
            throw new NotImplementedException();
        }
    }
}