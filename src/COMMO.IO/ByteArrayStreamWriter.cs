using System;
using System.Text;

namespace COMMO.IO
{
    public class ByteArrayStreamWriter
    {
        public ByteArrayStreamWriter(ByteArrayStream stream)
        {
            BaseStream = stream;
        }

        public ByteArrayStream BaseStream { get; }

        public void Write(byte value) => BaseStream.WriteByte(value);

		public void Write(bool value) => Write(BitConverter.GetBytes(value));

		public void Write(short value) => Write(BitConverter.GetBytes(value));

		public void Write(ushort value) => Write(BitConverter.GetBytes(value));

		public void Write(int value) => Write(BitConverter.GetBytes(value));

		public void Write(uint value) => Write(BitConverter.GetBytes(value));

		public void Write(long value) => Write(BitConverter.GetBytes(value));

		public void Write(ulong value) => Write(BitConverter.GetBytes(value));

		public void Write(string value)
        {
            Write( (ushort)value.Length );

            Write( Encoding.Default.GetBytes(value) );
        }

		public void Write(byte[] buffer) => BaseStream.Write(buffer, 0, buffer.Length);
	}
}