using COMMO.Common.Structures;

namespace COMMO.Common.Objects
{
    public class TeleportItem : Item
    {
        public TeleportItem(ItemMetadata metadata) : base(metadata)
        {

        }

        public Position Position { get; set; }
    }
}