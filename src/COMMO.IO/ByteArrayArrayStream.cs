using System;

namespace COMMO.IO
{
    public class ByteArrayArrayStream : ByteArrayStream
    {
        private readonly byte[] _bytes;

        public ByteArrayArrayStream(byte[] bytes)
        {
            _bytes = bytes;
        }

        public override byte ReadByte()
        {
            byte value = _bytes[_position];

            Seek(Origin.Current, 1);

            return value;
        }

        public override void Read(byte[] buffer, int offset, int count)
        {
            Buffer.BlockCopy(_bytes, _position, buffer, offset, count);

            Seek(Origin.Current, count);
        }

        public override void WriteByte(byte value)
        {
            _bytes[_position] = value;

            Seek(Origin.Current, 1);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Buffer.BlockCopy(buffer, offset, _bytes, _position, count);

            Seek(Origin.Current, count);
        }
    }
}