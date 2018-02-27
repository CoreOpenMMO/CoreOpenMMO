using System;

namespace COMMO.Network.Cryptography {
	public static class XTea
    {
		private const uint BakedSum = 0xC6EF3720; //(Delta * Rounds)
		private const uint Delta = 0x9E3779B9;
        private const uint Rounds = 32;

		public static byte[] EncryptXtea(byte[] msg, uint[] key) {
			if (key.Length < 4)
				throw new Exception("Trying to Encrypt with XTEA using an invalid key.");

			if (msg.Length == 0)
				throw new Exception("Trying to Encrypt an empty message with XTEA.");

			if (msg.Length % 8 != 0)
				throw new Exception("Trying to Encrypt a message that is not multiple of 8 bytes with XTEA.");

			uint count = 0;
			uint x_sum = 0;
			uint temporary = 0;
			var intbuffer = new BufferRepresentation(msg);
			for (var pos = 0; pos < msg.Length / 4; pos += 2, x_sum = 0) {
				//Run rounds of XTea

				count = Rounds;
				while (count-- > 0) {
					temporary = intbuffer.UIntBuffer[pos + 1];
					intbuffer.UIntBuffer[pos] += (((temporary << 4) ^ (temporary >> 5)) + temporary) ^ (x_sum + key[x_sum & 3]);

					x_sum += Delta;

					temporary = intbuffer.UIntBuffer[pos];
					intbuffer.UIntBuffer[pos + 1] += (((temporary << 4) ^ (temporary >> 5)) + temporary) ^ (x_sum + key[(x_sum >> 11) & 3]);
				}
			}

			return intbuffer.ByteBuffer;
		}

        public static byte[] DecryptXtea(byte[] encmsg, uint[] key) {
			if (key.Length < 4)
				throw new Exception("Trying to Decrypt with XTEA using an invalid key.");

			if (encmsg.Length == 0)
				throw new Exception("Trying to Decrypt an empty message with XTEA.");

			if (encmsg.Length % 8 != 0)
				throw new Exception("Trying to Decrypt a message that is not multiple of 8 bytes with XTEA.");

			uint count = 0;
			uint x_sum = BakedSum;
			uint temporary = 0;
			var intbuffer = new BufferRepresentation(encmsg);
			for (var pos = 0; pos < encmsg.Length / 4; pos += 2, x_sum = BakedSum) {
			    //Run rounds of XTea

				count = Rounds;
				while (count-- > 0) {
					temporary = intbuffer.UIntBuffer[pos];
					intbuffer.UIntBuffer[pos + 1] -= (((temporary << 4) ^ (temporary >> 5)) + temporary) ^ (x_sum + key[(x_sum >> 11) & 3]);
					
					x_sum -= Delta;

					temporary = intbuffer.UIntBuffer[pos + 1];
					intbuffer.UIntBuffer[pos] -= (((temporary << 4) ^ (temporary >> 5)) + temporary) ^ (x_sum + key[x_sum & 3]);
				}
			}

			return intbuffer.ByteBuffer;
		}
	}
}

