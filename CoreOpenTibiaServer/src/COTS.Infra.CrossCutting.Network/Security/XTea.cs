using COTS.Infra.CrossCutting.Network.Enums;
using System;
using System.Linq;

namespace COTS.Infra.CrossCutting.Network.Security
{
    public static class XTea
    {
        private const uint _delta = 0x9e3779b9;
        private const uint _rounds = 32;

        /// <summary>
        /// Decryption method for Tibia XTea.
        /// Buffer based
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static unsafe bool DecryptXtea(ref byte[] buffer, ref int length, int index, uint[] key)
        {
            if (length <= index || (length - index) % 8 > 0 || key == null)
                return false;

            fixed (byte* bufferPtr = buffer)
            {
                uint* words = (uint*)(bufferPtr + index);
                int msgSize = length - index;

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

            length = (BitConverter.ToUInt16(buffer, index) + 2 + index);
            return true;
        }

		/*public static byte[] XteaEncrypt(byte[] bytes, uint[] key) {
			uint x_sum = 0;
			uint x_count = 0;
			var indexCurr = 0;
			var indexCurrPlusOne = 0;
			uint current = 0;
			uint currentPlusOne = 0;

			var size = bytes.Length;
			var pad = size % 8;
			if (pad > 0) {
				msg.AddPaddingBytes(8 - pad);
			}
		}*/

		public static byte[] XteaEncrypt(byte[] msg, uint[] key) {
			if (key.Length < 4)
				throw new Exception("Trying to Encrypt with XTEA with an invalid key.");

			if (msg.Length % 8 != 0)
				throw new Exception("Trying to Encrypt a message that is not multiple of 8 bytes with XTEA.");

			/*key[0] = 3442030272;
			key[1] = 2364789040;
			key[2] = 1503299581;
			key[3] = 3670909886;*/

			uint x_sum = 0;
			uint x_count = 0;
			uint temporary = 0;
			var intbuffer = new BufferRepresentation(msg);
			for (var pos = 0; pos < msg.Length / 4; pos += 2, x_sum = 0) {
				//Run rounds of XTea
				for (var count = _rounds; count > 0; count--) {
					temporary = intbuffer.UIntBuffer[pos + 1];
					intbuffer.UIntBuffer[pos] += (temporary << 4 ^ temporary >> 5) + temporary ^ x_sum + key[x_sum & 3];

					x_sum += _delta;

					temporary = intbuffer.UIntBuffer[pos];
					intbuffer.UIntBuffer[pos + 1] += (temporary << 4 ^ temporary >> 5) + temporary ^ x_sum + key[x_sum >> 11 & 3];
				}
			}

			//Array.Copy(msgCryptografada, msg, headerSize);
			return intbuffer.ByteBuffer;
		}

		public static unsafe bool EncryptXtea(ref OutputMessage msg, uint[] key)
        {
            if (key == null)
                return false;

			key[0] = 3442030272;
			key[1] = 2364789040;
			key[2] = 1503299581;
			key[3] = 3670909886;

			uint x_sum = 0;
			uint x_count = 0;
			var indexCurr = 0;
			var indexCurrPlusOne = 0;
			uint current = 0;
			uint currentPlusOne = 0;

			var pad = msg.Length % 8;
            if (pad > 0) {
                msg.AddPaddingBytes(8 - pad);
            }

			/*fixed (byte* bufferPtr = msg.Buffer) {
				uint* words = (uint*)(bufferPtr + msg.HeaderPosition);

				for (int pos = 0; pos < msg.Length / 4; pos += 2) {
					x_sum = 0; x_count = _rounds;

					while (x_count-- > 0) {
						words[pos] += (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ x_sum + key[x_sum & 3];
						x_sum += _delta;
						words[pos + 1] += (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ x_sum + key[x_sum >> 11 & 3];
					}
					Console.WriteLine("=D");
				}
			}*/

			//byte[] headerLess = msg.Buffer.Skip(6).ToArray();
			//var intbuffer = new BufferRepresentation(headerLess);

			for (var pos = 0; pos < msg.Length / 4; pos += 2, x_sum = 0) {
				//Run rounds of XTea
				for (var count = _rounds; count > 0; count--) {
					indexCurr = msg.HeaderPosition + (pos * sizeof(uint));
					indexCurrPlusOne = msg.HeaderPosition + ((pos + 1) * sizeof(uint));

					current = msg.GetUInt32FromIndex(indexCurr);
					currentPlusOne = msg.GetUInt32FromIndex(indexCurrPlusOne);

					if (count == 32) {
						Console.WriteLine("Iteration POS: " + pos);
						Console.WriteLine("Pos: " + current);
						Console.WriteLine("Pos+1: " + currentPlusOne);
					}

					msg.OverwriteBytes(indexCurr,
						BitConverter.GetBytes(
							current +
							(
								(currentPlusOne << 4 ^ currentPlusOne >> 5) + currentPlusOne ^ x_sum + key[x_sum & 3]
							)
						)
					);

					x_sum += _delta;

					current = msg.GetUInt32FromIndex(indexCurr);
					currentPlusOne = msg.GetUInt32FromIndex(indexCurrPlusOne);

					if (count == 32) {
						Console.WriteLine("PosSum_Pos: " + current);
						Console.WriteLine("PosSum_Pos+1: " + currentPlusOne);
					}

					msg.OverwriteBytes(indexCurrPlusOne,
						BitConverter.GetBytes(
							currentPlusOne +
							(
								(current << 4 ^ current >> 5) + current ^ x_sum + key[x_sum >> 11 & 3]
							)
						)
					);
				}
				Console.WriteLine("=D");
			}

			return true;
        }

        /// <summary>
        /// Same as method above for Decrypt, but uses NetworkMessage.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static unsafe bool DecryptXtea(NetworkMessage msg, uint[] key)
        {
            if ((msg.Length - msg.Position) % 8 > 0 || key == null)
                return false;

            fixed (byte* bufferPtr = msg.Buffer)
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

            return true;
        }
    }
}

