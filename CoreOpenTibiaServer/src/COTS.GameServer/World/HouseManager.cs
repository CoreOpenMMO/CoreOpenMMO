using System;
using System.Collections.Generic;

namespace COTS.GameServer.World {

    public sealed class HouseManager {
        private static readonly HouseManager _instance = new HouseManager();
        public static HouseManager Instance => _instance;

        private HouseManager() {
        }

        private readonly Dictionary<UInt32, House> _houses = new Dictionary<UInt32, House>();

        public House CreateHouseOrGetReference(UInt32 houseId) {
            if (_houses.TryGetValue(houseId, out var house)) {
                return house;
            } else {
                house = new House(houseId);
                _houses[houseId] = house;
                return house;
            }
        }
    }
}