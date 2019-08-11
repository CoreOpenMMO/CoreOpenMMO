namespace COMMO.Utilities {
	using System.Collections.Generic;

	/// <summary>
	/// Contains extension methods for arrays.
	/// </summary>
	public static class ArrayExtensions {

		/// <summary>
		/// This is a convenience method to get a generic enumrator from an array.
		/// </summary>
		public static IEnumerator<T> GetGenericEnumerator<T>(this T[] array) => ((IEnumerable<T>) array).GetEnumerator();
	}
}
