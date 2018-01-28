namespace COTS.GameServer.Items {

    public class SharedItem {
        public SharedItemFlags flags;

        public ushort id;
        public ushort clientId;
        public ushort wareId;
        public ushort speed = 0;

        public byte lightLevel = 0;
        public byte lightColor = 0;
        public byte alwaysOnTopOrder = 0;

        public SharedItemAbilities abilities;

        public string name;
        public string article;
        public string pluralName;
        public string description;
        public string type; // Maybe not string. But for now ok
        public string floorChange; // Maybe not string. But for now ok
        public string effect; // Maybe not string. But for now ok
        public string field; // Maybe not string. But for now ok
        public string fluidSource; // Maybe not string. But for now ok
        public string weaponType; // Weapons HERE??? // Maybe not string. But for now ok
        public string shootType; // Weapons HERE??? // Maybe not string. But for now ok
        public string ammoType; // Weapons HERE??? // Maybe not string. But for now ok
        public string partnerDirection; // Maybe not string. But for now ok
        public string corpseType; // Maybe not string. But for now ok
        public string slotType; // Maybe not string. But for now ok

        public byte containerSize = 0; // 255 should be a good max to containzer size

        public ushort decaytTo = 0;
        public ushort rotateTo = 0;
        public ushort destroyTo = 0;
        public ushort writeOnceItemId = 0;
        public ushort maleSleeperId = 0;
        public ushort femaleSleeperId = 0;
        public ushort maxTextLength = 0;
        public ushort attack = 0; // Weapons HERE???
        public ushort defense = 0; // Weapons HERE???
        public ushort maxHitChance = 0; // Weapons HERE???
        public ushort range = 0; // Weapons HERE???
        public ushort levelDoor = 0; // Whats? But Okay. // ushort should be okay if level <= 65k

        public uint weight = 0; // Maybe not so much
        public uint duration = 0; // In ms, ushort is not big enough AAAAAAAAAAA. Maybe divide by 10 and make as seconds
        public uint damage = 0;
        public uint damageTicks = 0;
        public uint damageCount = 0; // 65k should be enough. Change it to ushort
        public uint damageStart = 0; // Not sure what is. Maybe 65k is enough too

        public bool isUseable = false; // default true?
        public bool isPickupable = false; // default true?
        public bool isMoveable = false; // default true?
        public bool isStackable = false; // default true?
        public bool isVertical = false; // default true?
        public bool isHorizontal = false; // default true?
        public bool isHangable = false;
        public bool isWriteable = false;
        public bool isRepleaceable = false; // Maybe the default is true
        public bool isRotatable = false;
        public bool isAnimation = false;
        public bool isReadable = false;
        public bool hasHeight = false; // Maybe hasElevation
        public bool allowDistRead = false;
        public bool allowPickupable = false;
        public bool blockSolid = false;
        public bool blockProjectile = false;
        public bool blockPathFind = false;
        public bool walkStack = true;
        public bool alwaysOnTop = true;
        public bool lookTrough = false;
        public bool forceUse = false;

        public SharedItem(SharedItemFlags flags = 0) {
            this.flags = flags;
            initFlags();
            abilities.reset();
        }

        private bool hasFlag(SharedItemFlags flag) {
            return (flags & flag) != 0;
        }

        private void initFlags() {
            isUseable = hasFlag(SharedItemFlags.USEABLE);
            isPickupable = hasFlag(SharedItemFlags.PICKUPABLE);
            isMoveable = hasFlag(SharedItemFlags.MOVEABLE);
            isStackable = hasFlag(SharedItemFlags.STACKABLE);
            isVertical = hasFlag(SharedItemFlags.VERTICAL);
            isHorizontal = hasFlag(SharedItemFlags.HORIZONTAL);
            isHangable = hasFlag(SharedItemFlags.HANGABLE);
            isRotatable = hasFlag(SharedItemFlags.ROTATABLE);
            isAnimation = hasFlag(SharedItemFlags.ANIMATION);
            isReadable = hasFlag(SharedItemFlags.READABLE);

            hasHeight = hasFlag(SharedItemFlags.HAS_HEIGHT);

            allowDistRead = hasFlag(SharedItemFlags.ALLOWDISTREAD);
            alwaysOnTop = hasFlag(SharedItemFlags.ALWAYSONTOP);
            blockSolid = hasFlag(SharedItemFlags.BLOCK_SOLID);
            blockProjectile = hasFlag(SharedItemFlags.BLOCK_PROJECTILE);
            blockPathFind = hasFlag(SharedItemFlags.BLOCK_PATHFIND);
            forceUse = hasFlag(SharedItemFlags.FORCEUSE);
            lookTrough = hasFlag(SharedItemFlags.LOOKTHROUGH);
        }
    }

    public struct SharedItemAbilities {
        public uint healthGain;
        public uint healthTicks;
        public uint manaGain;
        public uint manaTicks;

        public bool manaShield;
        public bool preventDrop;
        public bool preventLoss;

        public void reset() {
            healthGain = healthTicks = manaGain = manaTicks = 0;
            manaShield = preventDrop = preventLoss = false;
        }
    }

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