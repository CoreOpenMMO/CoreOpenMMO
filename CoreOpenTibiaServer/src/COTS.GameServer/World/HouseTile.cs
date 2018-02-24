namespace COTS.GameServer.World {

    public sealed class HouseTile : Tile {
        public readonly House House;

        private HouseTile(ushort x, ushort y, byte z, House house)
            : base(x, y, z) {
            if (house == null)
                throw new System.ArgumentNullException(nameof(house));

            House = house;
        }

        public static HouseTile CreateTileAndAddItToHouse(ushort x, ushort y, byte z, House house) {
            var tile = new HouseTile(x: x, y: y, z: z, house: house);
            house.AddTile(tile);

            return tile;
        }
    }
}