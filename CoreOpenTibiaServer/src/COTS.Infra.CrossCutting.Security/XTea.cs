using System;

namespace COTS.Infra.CrossCutting.Security
{
    public static class XTea
    {
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
        
        /// <summary>
        /// Based on Output network message
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static unsafe bool EncryptXtea(OutputMessage msg, uint[] key)
        {
            if (key == null)
                return false;

            int pad = msg.Length % 8;
            if (pad > 0)
                msg.AddPaddingBytes(8 - pad);

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

