namespace COMMO.Utilities {
	using System;

	public static class UIntArrayExtensions {

		public static byte[] ToByteArray(this uint[] unsignedIntegers) {
			var temp = new byte[unsignedIntegers.Length * sizeof(uint)];

			for (var i = 0; i < unsignedIntegers.Length; i++) {
				Array.Copy(BitConverter.GetBytes(unsignedIntegers[i]), 0, temp, i * 4, 4);
			}

			return temp;
		}
	}
}
