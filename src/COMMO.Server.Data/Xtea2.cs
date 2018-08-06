using System;
using System.Collections.Generic;
using System.Text;

namespace COMMO.Server.Data
{
    public class Xtea2
    {
        public unsafe static bool EncryptXtea(NetworkMessage msg, uint[] key)
        {
            if (key == null)
                return false;

			Console.WriteLine($"XTEA: msg.Length = {msg.Length}");

            int pad = msg.Length % 8;

			Console.WriteLine($"XTEA: pad = {pad}");

            if (pad > 0)
			{ 
				Console.WriteLine($"XTEA: pad > 0");
				msg.AddPaddingBytes(8-pad);
				Console.WriteLine($"XTEA: padADD = {8-pad}");
				Console.WriteLine($"XTEA: msg.Length = {msg.Length}");
			}
                

            fixed (byte* bufferPtr = msg.Buffer)
            {
                uint* words = (uint*)(bufferPtr + msg.HeaderPosition);

                for (int pos = 0; pos < msg.Length / 4; pos += 2)
                {
                    uint x_sum = 0, x_delta = 0x9e3779b9, x_count = 32;

                    while (x_count-- > 0)
                    {
                        words[pos] += (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ x_sum
                            + key[x_sum & 3];
                        x_sum += x_delta;
                        words[pos + 1] += (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ x_sum
                            + key[x_sum >> 11 & 3];
                    }
                }
            }

            return true;
        }

        public unsafe static bool DecryptXtea(ref byte[] buffer, ref int length, int index, uint[] key)
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

        public unsafe static bool DecryptXtea(NetworkMessage msg, uint[] key)
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