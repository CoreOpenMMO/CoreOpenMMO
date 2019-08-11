using COMMO.Common.Structures;

namespace COMMO.Common.Objects
{
    public class MonsterMetadata
    {
        public string Name { get; set; }

        public ushort Health { get; set; }

        public ushort MaxHealth { get; set; }

        public Outfit Outfit { get; set; }

        public ushort Speed { get; set; }
    }
}