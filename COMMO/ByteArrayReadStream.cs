using System;

namespace COMMO {

	/// <summary>
	/// This class allows us to navigate a byte array in stream-like fashion.
	/// </summary>
	public sealed class ByteArrayReadStream {
		private readonly byte[] _array;
		public int Position { get; private set; }

		public ByteArrayReadStream(byte[] array, int position = 0) {
			if (array == null)
				throw new ArgumentNullException(nameof(array));
			if (position < 0 || position > array.Length)
				throw new ArgumentOutOfRangeException(nameof(position));

			_array = array;
			Position = position;
		}

		public bool IsOver => Position >= _array.Length;

		public int BytesLeftToRead() => _array.Length - Position;

		public byte PeakByte() => _array[Position];

		public byte ReadByte() {
			if (IsOver)
				throw new InvalidOperationException();

			var data = _array[Position];
			Position += sizeof(byte);

			return data;
		}

		public UInt16 ReadUInt16() {
			if (IsOver)
				throw new InvalidOperationException();

			var data = BitConverter.ToUInt16(_array, Position);
			Position += sizeof(UInt16);

			return data;
		}

		public UInt32 ReadUInt32() {
			if (IsOver)
				throw new InvalidOperationException();

			var data = BitConverter.ToUInt32(_array, Position);
			Position += sizeof(UInt32);

			return data;
		}

		public void Skip(int byteCount = 1) {
			if (byteCount <= 0)
				throw new ArgumentOutOfRangeException(nameof(byteCount));
			if (Position + byteCount >= _array.Length)
				throw new ArgumentOutOfRangeException(nameof(byteCount));

			Position += byteCount;
		}
	}
}