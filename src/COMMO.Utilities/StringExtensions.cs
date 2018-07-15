namespace COMMO.Utilities {
	using System;
	using System.Collections.Generic;
	using System.Text;

	public static class StringExtensions {

		/// <summary>
		/// Repeats the specified string n times.
		/// </summary>
		/// <param name="instr">The input string.</param>
		/// <param name="n">The number of times input string
		/// should be repeated.</param>
		/// <returns></returns>
		// http://weblogs.asp.net/gunnarpeipman/archive/2009/05/13/string-repeat-smaller-and-faster-version.aspx
		public static string Repeat(this string instr, int n) {
			if (string.IsNullOrEmpty(instr)) {
				return instr;
			}

			var result = new StringBuilder(instr.Length * n);
			return result.Insert(0, instr, n).ToString();
		}

		/// <summary>
		/// Converts a string to a byte array
		/// </summary>
		/// <returns></returns>
		public static byte[] ToByteArray(this string s) {
			var value = new List<byte>();
			foreach (var c in s) {
				value.Add(c.ToByte());
			}

			return value.ToArray();
		}

		/// <summary>Convert a string of hex digits (ex: E4 CA B2) to a byte array.</summary>
		/// <param name="s">The string containing the hex digits (with or without spaces).</param>
		/// <returns>Returns an array of bytes.</returns>
		public static byte[] ToBytesAsHex(this string s) {
			s = s.Replace(" ", string.Empty);
			var buffer = new byte[s.Length / 2];
			for (var i = 0; i < s.Length; i += 2) {
				buffer[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
			}

			return buffer;
		}

		/// <summary>
		/// Convert a string of hex digits to a printable string of characters.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToPrintableStringAsHex(this string value) {
			byte[] temp = value.ToBytesAsHex();
			var loc = string.Empty;
			for (var i = 0; i < temp.Length; i++) {
				loc += temp[i].ToPrintableChar();
			}

			return loc;
		}

		/// <summary>
		/// Converts a hex string to a integer
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static int ToIntAsHex(this string value) {
			byte[] bytes = value.ToBytesAsHex();
			if (bytes.Length >= 2) {
				return BitConverter.ToInt16(bytes, 0);
			}

			return int.MinValue;
		}
	}
}
