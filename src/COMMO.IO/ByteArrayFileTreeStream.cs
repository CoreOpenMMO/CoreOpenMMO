using System;
using System.IO;

namespace COMMO.IO
{
    public class ByteArrayFileTreeStream : ByteArrayBufferedStream
    {
        private const byte Escape = 0xFD;

        private const byte Start = 0xFE;

        private const byte End = 0xFF;

        public ByteArrayFileTreeStream(string path) : base( new FileStream(path, FileMode.Open) )
        {

        }

        public bool Child()
        {
            byte value = GetByte();

            if (value == Start)
            {
                return true;
            }

            Seek(Origin.Current, -1);

            return false;
        }

        public bool Next()
        {
            byte value = GetByte();

            if (value == End)
            {
                value = GetByte();

                if (value == Start)
                {
                    return true;
                }
            }

            Seek(Origin.Current, -1);

            return false;
        }

        public override byte ReadByte()
        {
            byte value = GetByte();

            if (value == Escape)
            {
                value = GetByte();
            }

            return value;
        }

        public override void Read(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                byte value = GetByte();

                if (value == Escape)
                {
                    value = GetByte();
                }

                buffer[i + offset] = value;
            }
        }

		public override void WriteByte(byte value) => throw new NotSupportedException();

		public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
	}
}