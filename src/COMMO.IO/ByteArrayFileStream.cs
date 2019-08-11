using System;
using System.IO;

namespace COMMO.IO
{
    public class ByteArrayFileStream : ByteArrayBufferedStream
    {
        public ByteArrayFileStream(string path) : base( new FileStream(path, FileMode.Open) )
        {

        }

		public override byte ReadByte() => GetByte();

		public override void Read(byte[] buffer, int offset, int count) => GetBytes(buffer, offset, count);

		public override void WriteByte(byte value) => throw new NotSupportedException();

		public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
	}
}