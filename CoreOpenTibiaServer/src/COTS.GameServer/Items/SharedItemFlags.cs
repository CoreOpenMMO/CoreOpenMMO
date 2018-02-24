namespace COTS.GameServer.Items {
	public enum SharedItemFlags : uint {
        BLOCK_SOLID = 1 << 0,
        BLOCK_PROJECTILE = 1 << 1,
        BLOCK_PATHFIND = 1 << 2,
        HAS_HEIGHT = 1 << 3,
        USEABLE = 1 << 4,
        PICKUPABLE = 1 << 5,
        MOVEABLE = 1 << 6,
        STACKABLE = 1 << 7,
        FLOORCHANGEDOWN = 1 << 8, // unused
        FLOORCHANGENORTH = 1 << 9, // unused
        FLOORCHANGEEAST = 1 << 10, // unused
        FLOORCHANGESOUTH = 1 << 11, // unused
        FLOORCHANGEWEST = 1 << 12, // unused
        ALWAYSONTOP = 1 << 13,
        READABLE = 1 << 14,
        ROTATABLE = 1 << 15,
        HANGABLE = 1 << 16,
        VERTICAL = 1 << 17,
        HORIZONTAL = 1 << 18,
        CANNOTDECAY = 1 << 19, // unused
        ALLOWDISTREAD = 1 << 20,
        UNUSED = 1 << 21, // unused
        CLIENTCHARGES = 1 << 22, /* deprecated */
        LOOKTHROUGH = 1 << 23,
        ANIMATION = 1 << 24,
        FULLTILE = 1 << 25, // unused
        FORCEUSE = 1 << 26,
    }
}