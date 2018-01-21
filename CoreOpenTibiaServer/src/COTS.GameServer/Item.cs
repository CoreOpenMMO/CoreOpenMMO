using System;

namespace COTS.GameServer {

    public class Item {
        public ItemGroup Group;
        public bool Stackable;
        public ushort ItemId;
        internal bool IsMoveable;
        internal bool IsGroundTile;
        public ushort Type;
        public ushort Count;

        public Item() {
        }

        private Item(ushort type, ushort count) {
            this.Type = type;
            this.Count = count;
        }

        public bool IsDepot { get; private set; }
        public bool IsContainer { get; private set; }
        public bool LoadedFromMap { get; internal set; }

        /// <summary>
        /// This method just converts "PvP" fields to persistent fields.
        /// I don't know why this is used... I'm just porting the C++ code.
        /// I'm guessing this is some sort of workaround to fix map editing issues.
        /// Like... If someone, while editing the map, creates a PvP field (which decays after some time)
        /// instead of a Persistent field (which lasts forever), this could should fix it.
        /// </summary>
        public static Item CreateFromId(ushort itemId) {
            switch ((ItemType)itemId) {
                // Fire fields
                case ItemType.FireFieldPvpLarge:
                return CreateItemFromTypeAndCount(type: (ushort)ItemType.FireFieldPersistentLarge, count: 0);

                case ItemType.FireFieldPvpMedium:
                return CreateItemFromTypeAndCount(type: (ushort)ItemType.FireFieldPersistentMedium, count: 0);

                case ItemType.FireFieldPvpSmall:
                return CreateItemFromTypeAndCount(type: (ushort)ItemType.FireFieldPersistentSmall, count: 0);

                // Energy fields
                case ItemType.EnergyFieldPvp:
                return CreateItemFromTypeAndCount(type: (ushort)ItemType.EnergyFieldPersistent, count: 0);

                // Poison field
                case ItemType.PoisonFieldPvp:
                return CreateItemFromTypeAndCount(type: (ushort)ItemType.PoisonFieldPersistent, count: 0);

                // Magic wall
                case ItemType.MagicWall:
                return CreateItemFromTypeAndCount(type: (ushort)ItemType.MagicWallPersistent, count: 0);

                // Wild growth
                case ItemType.WildGrowth:
                return CreateItemFromTypeAndCount(type: (ushort)ItemType.WildGrowthPersistent, count: 0);

                default:
                return CreateItemFromTypeAndCount(type: itemId, count: 0);
            }
        }

        public static Item CreateItemFromTypeAndCount(ushort type, ushort count) {
            var itemType = ItemManager.Instance.ItemsById[type];
            var itemCount = count;

            if (itemType.Group == ItemGroup.Deprecated)
                throw new DeprecatedItemGroupException();

            if (itemType.Stackable && count == 0)
                itemCount = 1;
            else
                itemCount = count;

#warning Finish implementing this
            return new Item(type, count);
        }

        internal void StartDecaying() {
            throw new NotImplementedException();
        }
    }
}