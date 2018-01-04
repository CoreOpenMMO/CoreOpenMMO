using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace COTS.Infra.CrossCutting.Security
{
    public static class CsharpExtensions
    {
        public static BigInteger ModInverse(this BigInteger a, BigInteger n)
        {
            BigInteger i = n, v = 0, d = 1;
            while (a > 0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0) v = (v + n) % n;
            return v;
        }

        public static void MemSet<T>(this T[] a, int index, T value, int num)
        {
            for (int i = 0; i < num; i++)
            {
                a[index + i] = value;
            }
        }

        public static void MemCpy<T>(this T[] destination, int index, T[] source, int num)
        {
            for (int i = 0; i < num; i++)
            {
                destination[index + i] = source[i];
            }
        }

        public static int MemCmp<T>(this T[] source, T[] target, int comparisonSize) where T : IComparable<T>
        {
            if (source.Length < comparisonSize || target.Length < comparisonSize)
                throw new ArgumentOutOfRangeException();

            for (int i = 0; i < comparisonSize; i++)
            {
                int comparison = source[i].CompareTo(target[i]);
                if (comparison != 0)
                    return comparison;
            }
            return 0;
        }

        public static string GetString(this BinaryReader reader)
        {
            ushort len = reader.ReadUInt16();
            return Encoding.Default.GetString(reader.ReadBytes(len));
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Shuffle<T>(this IList<T> list, int index, int count)
        {
            Random rng = new Random();

            if (index + count > list.Count)
                count = list.Count - index;

            int n = index + count;
            while (n > index)
            {
                n--;
                int k = rng.Next(index, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        #region Memory Stream Extension
        public static char[] ReadChars(this MemoryStream stream, int count)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, count);
            return Encoding.UTF8.GetChars(buffer);
        }

        public static bool ReadBoolean(this MemoryStream stream)
        {
            byte[] buffer = new byte[sizeof(bool)];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToBoolean(buffer, 0);
        }
        public static ushort ReadUInt16(this MemoryStream stream)
        {
            byte[] buffer = new byte[sizeof(ushort)];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToUInt16(buffer, 0);
        }
        public static int ReadInt32(this MemoryStream stream)
        {
            byte[] buffer = new byte[sizeof(int)];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToInt32(buffer, 0);
        }
        public static uint ReadUInt32(this MemoryStream stream)
        {
            byte[] buffer = new byte[sizeof(uint)];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToUInt32(buffer, 0);
        }
        public static float ReadFloat(this MemoryStream stream)
        {
            byte[] buffer = new byte[sizeof(float)];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToSingle(buffer, 0);
        }
        public static byte[] ReadBytes(this MemoryStream stream, int length)
        {
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }
        public static string GetString(this MemoryStream stream)
        {
            ushort len = stream.ReadUInt16();
            return Encoding.Default.GetString(stream.ReadBytes(len));
        }

        public static void WriteInt32(this MemoryStream stream, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }
        public static void WriteUInt16(this MemoryStream stream, ushort value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }
        public static void WriteUInt32(this MemoryStream stream, uint value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }
        public static void WriteFloat(this MemoryStream stream, float value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }
        public static void WriteBoolean(this MemoryStream stream, bool value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }
        #endregion
    }
}
