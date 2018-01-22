namespace COTS.GameServer.Items
{
    public class ItemManager
    {
        private static ItemManager _instance;
        public static ItemManager GetInstance()
        {
            if(_instance == null)
                _instance = new ItemManager();

            return _instance;
        }

        private ItemManager() {} // Defeat Instantiation

        private Item[] _items; // Should be and List or Map of Int32 and add by its UID
        private SharedItem[] _shared_items;

        public Item GetItem(ushort uid)
        {
            if(uid < 0 || uid >= _items.Length)
                return null;

            return _items[uid];
        }

        public SharedItem GetSharedItem(ushort id)
        {
            if(id < 0 || id > _shared_items.Length)
                return null;

            return _shared_items[id];
        }

        public bool PushItem(Item item)
        {
            if(item == null)
                return false;
            
            if(item.ID > _items.Length)
            { /* Resize _items to insert a new element */ }

            return (_items[item.ID] = item) == item;
        }

        public bool PushSharedItem(SharedItem s_item)
        {
            if(s_item == null)
                return false;

            if(s_item.ID > _shared_items.Length)
            { /* Resize _shared_items to insert a new element */}

            return (_shared_items[s_item.ID] = s_item) == s_item;
        }

        public bool DeleteItem(Item item)
        { // Finish this
            Item deletedItem;
            if(true) // _items.contains item
                deletedItem = null; // = _items.remove item
            
            return (deletedItem = null) == null;
        }

        public void LoadFromJSON(string path) {}
        public void LoadFromOTB(string path) {}
    }
}