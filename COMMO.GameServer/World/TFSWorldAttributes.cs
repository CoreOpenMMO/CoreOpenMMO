using System;

namespace COMMO.GameServer.World {

    public sealed class TFSWorldAttributes {
        public readonly string WorldDescription;
        public readonly string SpawnsFilename;
        public readonly string HousesFilename;

        public TFSWorldAttributes(
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