namespace COMMO.Utilities {

	public static class ByteExtensions {

		/// <summary>
		/// Convert a byte to a printable
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static char ToPrintableChar(this byte value) {
			if (value < 32 || value > 126) {
				return '.';
			}

			return (char)value;
		}
	}
}
