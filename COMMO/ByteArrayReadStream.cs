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

		public bool IsOver => 
			Position >= _array.Length;

		public int BytesLeftToRead => 
			_array.Length - Position;

		public byte GetByte() => 
			_array[Position];

		public byte ReadByte() {
			var data = _array[Position];
			Position += sizeof(byte);

			return data;
		}

		public ushort ReadUshort() {
			var data = BitConverter.ToUInt16(_array, Position);
			Position += sizeof(ushort);

			return data;
		}

		public uint ReadUint() {
			var data = BitConverter.ToUInt32(_array, Position);
			Position += sizeof(uint);

			return data;
		}

		public void Skip(ushort byteCount = 1) => 
			Position += byteCount;
	}
}