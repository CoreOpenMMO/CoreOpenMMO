namespace COMMO.Common.Structures
{
    public class Outfit
    {
        public Outfit(int itemId) : this( (ushort)itemId )
        {

        }

        public Outfit(ushort itemId)
        {
            ItemId = itemId;
        }

        public ushort ItemId { get; }

        public Outfit(int id, int head, int body, int legs, int feet, Addon addon) : this( (ushort)id, (byte)head, (byte)body, (byte)legs, (byte)feet, addon )
        {

        }

        public Outfit(ushort id, byte head, byte body, byte legs, byte feet, Addon addon)
        {
            Id = id;

            Head = head;

            Body = body;

            Legs = legs;

            Feet = feet;

            Addon = addon;
        }

        public ushort Id { get; }

        public byte Head { get; }

        public byte Body { get; }

        public byte Legs { get; }

        public byte Feet { get; }

        public Addon Addon { get; }

        public static bool operator ==(Outfit a, Outfit b)
        {
            if ( object.ReferenceEquals(a, b) )
            {
                return true;
            }

            if ( a is null || b is null)
            {
                return false;
            }

            return (a.ItemId == b.ItemId) && (a.Id == b.Id) && (a.Head == b.Head) && (a.Body == b.Body) && (a.Legs == b.Legs) && (a.Feet == b.Feet) && (a.Addon == b.Addon);
        }

		public static bool operator !=(Outfit a, Outfit b) => !(a == b);

		public override bool Equals(object outfit) => Equals(outfit as Outfit);

		public bool Equals(Outfit outfit)
        {
            if (outfit == null)
            {
                return false;
            }

            return (ItemId == outfit.ItemId) && (Id == outfit.Id) && (Head == outfit.Head) && (Body == outfit.Body) && (Legs == outfit.Legs) && (Feet == outfit.Feet) && (Addon == outfit.Addon);
        }

        public override int GetHashCode()
        {
            int hashCode = 17;

            hashCode = hashCode * 23 + ItemId.GetHashCode();

            hashCode = hashCode * 23 + Id.GetHashCode();

            hashCode = hashCode * 23 + Head.GetHashCode();

            hashCode = hashCode * 23 + Body.GetHashCode();

            hashCode = hashCode * 23 + Legs.GetHashCode();

            hashCode = hashCode * 23 + Feet.GetHashCode();

            hashCode = hashCode * 23 + Addon.GetHashCode();

            return hashCode;
        }

        public override string ToString()
        {
            if (ItemId == 0)
            {
                return "Id: " + Id + " Head: " + Head + " Body: " + Body + " Legs: " + Legs + " Feet: " + Feet + " Addons: " + Addon;
            }

            return "Item id: " + ItemId;
        }
    }
}