namespace COMMO.Common.Structures
{
    public class Light
    {
        public static readonly Light Day = new Light(250, 215);

        public static readonly Light Night = new Light(40, 215);
        
        public Light(byte level, byte color)
        {
            Level = level;

            Color = color;
        }

        public byte Level { get; }

        public byte Color { get; }

        public static bool operator ==(Light a, Light b)
        {
            if ( object.ReferenceEquals(a, b) )
            {
                return true;
            }

            if ( a is null || b is null)
	        {
                return false;
	        }

            return (a.Level == b.Level) && (a.Color == b.Color);
        }

		public static bool operator !=(Light a, Light b) => !(a == b);

		public override bool Equals(object light) => Equals(light as Light);

		public bool Equals(Light light)
        {
            if (light == null)
            {
                return false;
            }

            return (Level == light.Level) && (Color == light.Color);
        }

        public override int GetHashCode()
        {
            int hashCode = 17;

            hashCode = hashCode * 23 + Level.GetHashCode();

            hashCode = hashCode * 23 + Color.GetHashCode();

            return hashCode;
        }

		public override string ToString() => "Level: " + Level + " Color: " + Color;
	}
}