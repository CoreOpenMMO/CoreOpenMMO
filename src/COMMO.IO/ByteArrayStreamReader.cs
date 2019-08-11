using System;
using System.Text;

namespace COMMO.IO
{
    public class ByteArrayStreamReader
    {
        public ByteArrayStreamReader(ByteArrayStream stream)
        {
            BaseStream = stream;
        }

        public ByteArrayStream BaseStream { get; }

        public byte ReadByte() => BaseStream.ReadByte();

		public bool ReadBool() => BitConverter.ToBoolean(ReadBytes(1), 0);

		public short ReadShort() => BitConverter.ToInt16(ReadBytes(2), 0);

		public ushort ReadUShort() => BitConverter.ToUInt16(ReadBytes(2), 0);

		public int ReadInt() => BitConverter.ToInt32(ReadBytes(4), 0);

		public uint ReadUInt() => BitConverter.ToUInt32(ReadBytes(4), 0);

		public long ReadLong() => BitConverter.ToInt64(ReadBytes(8), 0);

		public ulong ReadULong() => BitConverter.ToUInt64(ReadBytes(8), 0);

		public string ReadString() => Encoding.Default.GetString(ReadBytes(ReadUShort()));

		public byte[] ReadBytes(int length)
        {
            byte[] buffer = new byte[length]; 

            BaseStream.Read(buffer, 0, buffer.Length);
            
            return buffer;
        }
    }
}