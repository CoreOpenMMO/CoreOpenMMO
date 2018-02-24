using COMMO.Network.Enums;
using System;
using System.Linq;

namespace COMMO.Network.Security
{
    public static class XTea
    {
		private const uint _bakedSum = 0xC6EF3720;
        private const uint _delta = 0x9e3779b9;
        private const uint _rounds = 32;

		public static byte[] EncryptXtea(byte[] msg, uint[] key) {
			if (key.Length < 4)
				throw new Exception("Trying to Encrypt with XTEA using an invalid key.");

			if (msg.Length % 8 != 0)
				throw new Exception("Trying to Encrypt a message that is not multiple of 8 bytes with XTEA.");

			uint count = 0;
			uint x_sum = 0;
			uint temporary = 0;
			var intbuffer = new BufferRepresentation(msg);
			for (var pos = 0; pos < msg.Length / 4; pos += 2, x_sum = 0) {
				//Run rounds of XTea

				count = _rounds;
				//for (count = _rounds; count > 0; count--) {
				while (count-- > 0) {
					temporary = intbuffer.UIntBuffer[pos + 1];
					intbuffer.UIntBuffer[pos] += (temporary << 4 ^ temporary >> 5) + temporary ^ x_sum + key[x_sum & 3];

					x_sum += _delta;

					temporary = intbuffer.UIntBuffer[pos];
					intbuffer.UIntBuffer[pos + 1] += (temporary << 4 ^ temporary >> 5) + temporary ^ x_sum + key[x_sum >> 11 & 3];
				}
			}

			return intbuffer.ByteBuffer;
		}

        public static byte[] DecryptXtea(byte[] encmsg, uint[] key) {
			if (key.Length < 4)
				throw new Exception("Trying to Decrypt with XTEA using an invalid key.");

			if (encmsg.Length % 8 != 0)
				throw new Exception("Trying to Decrypt a message that is not multiple of 8 bytes with XTEA.");

			uint count = 0;
			uint x_sum = 0;
			uint temporary = 0;
			var intbuffer = new BufferRepresentation(encmsg);
			for (var pos = 0; pos < encmsg.Length / 4; pos += 2, x_sum = _bakedSum) {
				//Run rounds of XTea

				count = _rounds;
				//for (count = _rounds; count > 0; count--) {
				while (count-- > 0) {
					temporary = intbuffer.UIntBuffer[pos];
					intbuffer.UIntBuffer[pos + 1] -= (temporary << 4 ^ temporary >> 5) + temporary ^ x_sum + key[x_sum >> 11 & 3];

					x_sum -= _delta;

					temporary = intbuffer.UIntBuffer[pos + 1];
					intbuffer.UIntBuffer[pos] += (temporary << 4 ^ temporary >> 5) + temporary ^ x_sum + key[x_sum & 3];
				}
			}

			return intbuffer.ByteBuffer;
		}
	}
}

