using System;

namespace COTS.GameServer.World.Loading {

    public sealed class WorldAttributes {
        public readonly string Description;
        public readonly string SpawnsFilename;
        public readonly string HousesFilename;

        public WorldAttributes(
            string description,
            string spawnsFilename,
            string housesFilename
            ) {
            if (description == null)
                throw new ArgumentNullException(nameof(description));
            if (spawnsFilename == null)
                throw new ArgumentNullException(nameof(spawnsFilename));
            if (housesFilename == null)
                throw new ArgumentNullException(nameof(housesFilename));

            Description = description;
            SpawnsFilename = spawnsFilename;
            HousesFilename = housesFilename;
        }
    }
}