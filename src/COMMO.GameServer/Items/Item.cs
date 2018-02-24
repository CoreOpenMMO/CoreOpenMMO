namespace COMMO.GameServer.Items {

    public class Item {
        public static int _itemAutoId = 0;
        private int _uid;
        public int GetUID => _uid;
        private ushort _id;

        public ushort ID {
            get => _id;
        }

        public Item(ushort id, bool bVirtual = false) {
            _id = 0; // Keep these for test until load from OTB be ready
            //_id = id; // Maybe should change to SharedID or Type or SharedType
            _uid = _itemAutoId++; // This will be the UID of this item in ItemManager

            //if(!bVirtual) // This is wrong. ItemManager will have an ADD method to add this by the UID
            //ItemManager.GetInstance().PushItem(this);
        }

        public bool IsMoveable() => false; // Should look for sharedItem with id

        // TODO: Attributes. Maybe not here because. Why a mailbox will have attributes??
    }
}