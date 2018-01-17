namespace COTS.GameServer.World {

    public sealed class HouseTile {
        public readonly ushort X;
        public readonly ushort Y;
        public readonly byte Z;
        public readonly House House;

        public HouseTile(ushort x, ushort y, byte z, House house) {
            if (house == null)
                throw new System.ArgumentNullException(nameof(house));

            X = x;
            Y = y;
            Z = z;
            House = house;
        }
    }
}