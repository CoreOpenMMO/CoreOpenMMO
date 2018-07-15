namespace COMMO.Utilities {
	using System;
	using System.Text;

	public static class ByteArrayExtensions {

		public static uint[] ToUInt32Array(this byte[] bytes) {
			if (bytes.Length % 4 > 0) {
				throw new Exception();
			}

			var temp = new uint[bytes.Length / 4];

			for (var i = 0; i < temp.Length; i++) {
				temp[i] = BitConverter.ToUInt32(bytes, i * 4);
			}

			return temp;
		}

		/// <summary>
		/// Convert an array of byte to a IP String (exemple: 127.0.0.1)
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToIpString(this byte[] value) {
			var ret = string.Empty;

			for (var i = 0; i < value.Length; i++) {
				ret += value[i] + ".";
			}

			return ret.TrimEnd('.');
		}

		/// <summary>
		/// Convert an array of byte to a printable string.
		/// </summary>
		/// <param name="bytes"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string ToPrintableString(this byte[] bytes, int start, int length) {
			var text = string.Empty;
			for (var i = start; i < start + length; i++) {
				text += bytes[i].ToPrintableChar();
			}

			return text;
		}

		/// <summary>Converts an array of bytes into a formatted string of hex digits (ex: E4 CA B2)</summary>
		/// <param name="data">The array of bytes to be translated into a string of hex digits.</param>
		/// <param name="start">The start position to convert.</param>
		/// <param name="length">The length of data to convert.</param>
		/// <returns>Returns a well formatted string of hex digits with spacing.</returns>
		public static string ToHexString(this byte[] data, int start, int length) {
			var sb = new StringBuilder(data.Length * 3);
			for (var i = start; i < start + length; i++) {
				sb.Append(Convert.ToString(data[i], 16).PadLeft(2, '0').PadRight(3, ' '));
			}

			return sb.ToString().ToUpper();
		}

		/// <summary>Converts an array of bytes into a formatted string of hex digits (ex: E4 CA B2)</summary>
		/// <param name="data">The array of bytes to be translated into a string of hex digits.</param>
		/// <returns>Returns a well formatted string of hex digits with spacing.</returns>
		public static string ToHexString(this byte[] data) {
			return data.ToHexString(0, data.Length);
		}
	}
}
