using System;

namespace COTS.GameServer {

    /// <summary>
    /// In various cases, the C++ implementation used memory mapped files and iterators.
    /// But in C# we load the entire file to memory as a byte array.
    /// This class allows us to navigate such byte arrays in a way that resembles
    /// the C++ iterator-based implementation.
    /// </summary>
    public sealed class ByteArrayReadStream {
        private readonly byte[] _array;
        public int Position { get; private set; }

        public ByteArrayReadStream(byte[] array, int position = 0) {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (position < 0 || position > array.Length)
                throw new ArgumentOutOfRangeException(nameof(position));

            this._array = array;
            this.Position = position;
        }

        public bool IsOver => Position >= _array.Length;

        public int BytesLeftToRead() {
            return _array.Length - Position;
        }

        public byte PeakByte() {
            return _array[Position];
        }

        public byte ReadByte() {
            var data = _array[Position];
            Position += sizeof(byte);

            return data;
        }

        public UInt16 ReadUInt16() {
            var data = BitConverter.ToUInt16(_array, Position);
            Position += sizeof(UInt16);

            return data;
        }

        public UInt32 ReadUInt32() {
            var data = BitConverter.ToUInt32(_array, Position);
            Position += sizeof(UInt32);

            return data;
        }

        public void Skip(int byteCount = 1) {
            Position += byteCount;
        }
    }
}