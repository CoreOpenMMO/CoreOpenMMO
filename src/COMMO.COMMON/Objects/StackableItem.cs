namespace COMMO.Common.Objects
{
    public class StackableItem : Item
    {
        public StackableItem(ItemMetadata metadata) : base(metadata)
        {

        }

        public byte Count { get; set; }
    }
}