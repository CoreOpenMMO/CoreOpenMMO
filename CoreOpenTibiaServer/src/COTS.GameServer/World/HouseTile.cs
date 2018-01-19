namespace COTS.GameServer.World {

    public sealed class HouseTile : Tile {
        public readonly House House;

        public HouseTile(ushort x, ushort y, byte z, House house)
            : base(x, y, z) {
            if (house == null)
                throw new System.ArgumentNullException(nameof(house));

            House = house;
        }
    }
}