using System.Collections.Generic;

namespace COTS.GameServer.World
{
    public sealed class Tile
    {
        public readonly Stack<Item> Items;
        public readonly Stack<PlayerCharacter> PlayerCharacters;

    }
}
