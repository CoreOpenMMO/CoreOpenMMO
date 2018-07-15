namespace COMMO.Utilities {
	using System;

	public static class IntExtensions {

		/// <summary>
		/// Converts a integer to a hex string
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToHexString(this int value) {
			var temp = BitConverter.GetBytes(value);
			return temp.ToHexString();
		}
	}
}
