namespace COTS.GameServer.Items
{
    public class SharedItem
    {
        private ushort _id;
        public ushort ID
        {
            get {return _id;}
        }

        private ushort _clientId;
        public ushort ClientID
        {
            get {return _clientId;}
        }

        public string name;
        public string article;
        public string pluralName;
        public string description;

        public bool pickupable;
        
        public SharedItemAttributes attributes;

        public SharedItem(ushort id, bool bVirtual = false)
        {
            _id = id;

            if(!bVirtual)
                ItemManager.GetInstance().PushSharedItem(this);
        }
    }

    struct SharedItemAttributes
    {
        uint healthGain;
	    uint healthTicks;
	    uint manaGain;
	    uint manaTicks;

        public void reset()
        {
            healthGain = healthTicks = manaGain = manaTicks = 0;
        }
    }
}