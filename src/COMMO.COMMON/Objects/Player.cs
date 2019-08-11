namespace COMMO.Common.Objects
{
    public class Player : Creature
    {
        public Player()
        {
            Inventory = new Inventory(this);

            Experience = 0;

            Level = 1;

            LevelPercent = 0;

            Mana = 50;

            MaxMana = 50;

            Soul = 100;

            Capacity = 10000;

            Stamina = 42 * 60;

            CanReportBugs = true;
        }

        public Inventory Inventory { get; }

        private IClient _client;

        public IClient Client {
			get => _client;
			set {
				if (value != _client) {
					var current = _client;

					_client = value;

					if (value == null) {
						current.Player = null;
					}
					else {
						_client.Player = this;
					}
				}
			}
		}

		public uint Experience { get; set; }

        public ushort Level { get; set; }

        public byte LevelPercent { get; set; }

        public ushort Mana { get; set; }

        public ushort MaxMana { get; set; }

        public byte Soul { get; set; }

        public uint Capacity { get; set; }

        public ushort Stamina { get; set; }

        public bool CanReportBugs { get; set; }
    }
}