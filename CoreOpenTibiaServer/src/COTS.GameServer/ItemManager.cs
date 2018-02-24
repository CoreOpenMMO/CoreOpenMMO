namespace COTS.GameServer {

    public sealed class ItemManager {

        public static class Encoding {
            public const uint SupportedMajorVersion = 3;
            public const uint SupportedMinorVersion = 62;
        }

        public readonly Item[] ItemsById;

        private static readonly ItemManager _instance = new ItemManager();
        public static ItemManager Instance => _instance;

        private ItemManager() {
        }
    }
}