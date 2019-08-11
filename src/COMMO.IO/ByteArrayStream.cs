namespace COMMO.IO
{
    public abstract class ByteArrayStream
    {
        protected int _position;

		public int Position => _position;

		protected int _length;

		public int Length => _length;

		public virtual void Seek(Origin origin, int offset)
        {
            switch (origin)
            {
                case Origin.Begin:

                    _position = offset;

                    break;

                case Origin.Current:

                    _position += offset;

                    break;
            }

            if (_position > _length)
            {
                _length = _position;
            }
        }

        public abstract byte ReadByte();

        public abstract void Read(byte[] buffer, int offset, int count);

        public abstract void WriteByte(byte value);

        public abstract void Write(byte[] buffer, int offset, int count);
    }
}