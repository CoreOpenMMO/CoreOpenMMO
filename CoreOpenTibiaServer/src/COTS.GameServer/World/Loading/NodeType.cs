namespace COTS.GameServer.World.Loading {

    public enum NodeType : byte {
        RootVersion1 = 1,
        MapData = 2,
        ItemDefinition = 3,
        TileArea = 4,
        Tile = 5,
        Item = 6,
        TileSquare = 7,
        TileReference = 8,
        Spawn = 9,
        SpawnArea = 10,
        Monster = 11,
        TownCollection = 12,
        Town = 13,
        HouseTile = 14,
        WayPointCollection = 15,
        WayPoint = 16
    }
}