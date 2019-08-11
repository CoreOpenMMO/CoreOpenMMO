using COMMO.Common.Structures;

namespace COMMO.Common.Objects
{
    public class Item : IContent
    {
        public Item(ItemMetadata metadata)
        {
            Metadata = metadata;
        }

        public ItemMetadata Metadata { get; }

        public TopOrder TopOrder => Metadata.TopOrder;

		public IContainer Container { get; set; }

        public IContainer GetParent()
        {
            var container = Container;

            while (container is IContent content)
            {
                container = content.Container;
            }

            return container;
        }

        public bool IsChildOfParent(IContent parent)
        {
            IContent item = this;

            while (item != null)
            {
                if (item == parent)
                {
                    return true;
                }

                item = item.Container as IContent;
            }

            return false;
        }
    }
}