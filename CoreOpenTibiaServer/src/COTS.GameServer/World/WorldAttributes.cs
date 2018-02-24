using System;

namespace COTS.GameServer.World {

    public sealed class WorldAttributes {
        public readonly string WorldDescription;
        public readonly string SpawnsFilename;
        public readonly string HousesFilename;

        public WorldAttributes(
            string worldDescription,
            string spawnsFilename,
            string housesFilename
            ) {
            if (worldDescription == null)
                throw new ArgumentNullException(nameof(worldDescription));
            if (spawnsFilename == null)
                throw new ArgumentNullException(nameof(spawnsFilename));
            if (housesFilename == null)
                throw new ArgumentNullException(nameof(housesFilename));

            WorldDescription = worldDescription;
            SpawnsFilename = spawnsFilename;
            HousesFilename = housesFilename;
        }
    }
}