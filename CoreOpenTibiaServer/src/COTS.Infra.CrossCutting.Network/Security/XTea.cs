using COTS.Infra.CrossCutting.Network.Enums;
using System;
using System.Linq;

namespace COTS.Infra.CrossCutting.Network.Security
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

        public static unsafe byte[] DecryptXtea(byte[] encmsg, uint[] key) {
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
		/*fixed (byte* bufferPtr = msg.Buffer)
		{
			uint* words = (uint*)(bufferPtr + msg.Position);
			int msgSize = msg.Length - msg.Position;

			for (int pos = 0; pos < msgSize / 4; pos += 2)
			{
				uint x_count = 32, x_sum = 0xC6EF3720, x_delta = 0x9E3779B9;

				while (x_count-- > 0)
				{
					words[pos + 1] -= (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ x_sum
						+ key[x_sum >> 11 & 3];
					x_sum -= x_delta;
					words[pos] -= (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ x_sum
						+ key[x_sum & 3];
				}
			}
		}

		return true;*/


		/*public static unsafe bool DecryptXtea(NetworkMessage msg, uint[] key) {
			if ((msg.Length - msg.Position) % 8 > 0 || key == null)
				return false;

			fixed (byte* bufferPtr = msg.Buffer) {
				uint* words = (uint*)(bufferPtr + msg.Position);
				int msgSize = msg.Length - msg.Position;

				for (int pos = 0; pos < msgSize / 4; pos += 2) {
					uint x_count = 32, x_sum = 0xC6EF3720, x_delta = 0x9E3779B9;

					while (x_count-- > 0) {
						words[pos + 1] -= (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ x_sum
							+ key[x_sum >> 11 & 3];
						x_sum -= x_delta;
						words[pos] -= (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ x_sum
							+ key[x_sum & 3];
					}
				}
			}

			return true;
		}*/

		public static unsafe bool OldDecryptXtea(ref byte[] buffer, ref int length, int index, uint[] key) {
			if (length <= index || (length - index) % 8 > 0 || key == null)
				return false;

			fixed (byte* bufferPtr = buffer) {
				uint* words = (uint*)(bufferPtr + index);
				int msgSize = length - index;

				for (int pos = 0; pos < msgSize / 4; pos += 2) {
					uint x_count = 32, x_sum = 0xC6EF3720, x_delta = 0x9E3779B9;

					while (x_count-- > 0) {
						words[pos + 1] -= (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ x_sum
							+ key[x_sum >> 11 & 3];
						x_sum -= x_delta;
						words[pos] -= (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ x_sum
							+ key[x_sum & 3];
					}
				}
			}

			length = (BitConverter.ToUInt16(buffer, index) + 2 + index);
			return true;
		}
	}
}

