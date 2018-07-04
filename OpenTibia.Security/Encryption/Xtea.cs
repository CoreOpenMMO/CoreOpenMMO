// <copyright file="Xtea.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Security.Encryption
{
    using System;

    public static class Xtea
    {
        public static unsafe bool Encrypt(ref byte[] buffer, ref int length, int index, uint[] key)
        {
            if (key == null)
            {
                return false;
            }

            int msgSize = length - index;

            int pad = msgSize % 8;
            if (pad > 0)
            {
                msgSize += 8 - pad;
                length = index + msgSize;
            }

            fixed (byte* bufferPtr = buffer)
            {
                uint* words = (uint*)(bufferPtr + index);

                for (int pos = 0; pos < msgSize / 4; pos += 2)
                {
                    uint xSum = 0, xDelta = 0x9e3779b9, xCount = 32;

                    while (xCount-- > 0)
                    {
                        words[pos] += (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ xSum
                            + key[xSum & 3];
                        xSum += xDelta;
                        words[pos + 1] += (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ xSum
                            + key[xSum >> 11 & 3];
                    }
                }
            }

            return true;
        }

        public static unsafe bool Decrypt(ref byte[] buffer, ref int length, int index, uint[] key)
        {
            if (length <= index || (length - index) % 8 > 0 || key == null)
            {
                return false;
            }

            fixed (byte* bufferPtr = buffer)
            {
                uint* words = (uint*)(bufferPtr + index);
                int msgSize = length - index;

                for (int pos = 0; pos < msgSize / 4; pos += 2)
                {
                    uint xCount = 32, xSum = 0xC6EF3720, xDelta = 0x9E3779B9;

                    while (xCount-- > 0)
                    {
                        words[pos + 1] -= (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ xSum
                            + key[xSum >> 11 & 3];
                        xSum -= xDelta;
                        words[pos] -= (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ xSum
                            + key[xSum & 3];
                    }
                }
            }

            length = BitConverter.ToUInt16(buffer, index) + 2 + index;
            return true;
        }
    }
}
