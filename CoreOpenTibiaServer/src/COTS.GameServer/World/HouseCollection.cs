using System;
using System.Collections.Generic;

namespace COTS.GameServer.World {

    public sealed class HouseCollection {
        private static readonly HouseCollection _instance = new HouseCollection();

        private readonly Dictionary<UInt32, House> _houses = new Dictionary<UInt32, House>();

        public static HouseCollection Instance => _instance;

        public House AddHouse(UInt32 id) {
            var house = new House(id);
            _houses.Add(id, house);

            return house;
        }
    }
}