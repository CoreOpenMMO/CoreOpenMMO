using System;

namespace COMMO.Network {

	public static class Checksum {
		/// <see cref="https://stackoverflow.com/questions/927277/why-modulo-65521-in-adler-32-checksum-algorithm"/>
		private const ushort AdlerModule = 65521;

		public static uint Adler32(ReadOnlySpan<byte> data) {
			uint a = 1;
			uint b = 0;

			for (int i = 0; i < data.Length; i++) {
				a = (a + data[i]) % AdlerModule;
				b = (b + a) % AdlerModule;
			}

			return (b << 16) | a;
		}
	}
}