namespace COMMO.Utilities.HashHelper {

	/// <summary>
	/// This class contains extensions methods meant to
	/// simplify the generation of hashcodes.
	/// </summary>
	public static class HashHelper {
		public const int Start = 1610612741;

		/// <summary>
		/// Combines the current hashcode with the hashcode of another object.
		/// </summary>
		public static int CombineHashCode<T>(this int hashCode, T arg) {
			unchecked {
				return 16777619 * hashCode + arg.GetHashCode();
			}
		}
	}
}
