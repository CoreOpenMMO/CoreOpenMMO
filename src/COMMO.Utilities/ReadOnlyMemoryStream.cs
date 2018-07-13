namespace COMMO.Utilities {
	using System;

	/// <summary>
	/// This struct is used to allow stream-like reading of <see cref="Span{T}"/>.
	/// </summary>
	public ref struct ReadOnlyMemoryStream {
		private readonly ReadOnlySpan<Byte> _buffer;
		public int Position { get; private set; }

		/// <summary>
		/// Creates a new instace of this class
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="position"></param>
		public ReadOnlyMemoryStream(ReadOnlySpan<Byte> buffer, int position = 0) {
			if (position < 0 || position > buffer.Length)
				throw new ArgumentOutOfRangeException(nameof(position));

			_buffer = buffer;
			Position = position;
		}

		/// <summary>
		/// Returns true if this instance can read at least 1 more byte.
		/// Returns false otherwise.
		/// </summary>
		public bool IsOver => Position >= _buffer.Length;

		/// <summary>
		/// Returns the number of bytes that can still be read.
		/// </summary>
		public int BytesLeftToRead => _buffer.Length - Position;

		/// <summary>
		/// Returns the value currently pointed by the stream, without moving
		/// the stream forward.
		/// </summary>
		public byte PeakByte() {
			if (IsOver)
				throw new InvalidOperationException();

			return _buffer[Position];
		}

		/// <summary>
		/// Reads 1 byte from the stream.
		/// </summary>
		public byte ReadByte() {
			if (IsOver)
				throw new InvalidOperationException();

			var data = _buffer[Position];
			Position += sizeof(byte);
			return data;
		}

		/// <summary>
		/// Reads two bytes from the stream and parses them as a UInt16.
		/// </summary>
		public UInt16 ReadUInt16() {
			if (BytesLeftToRead < sizeof(UInt16))
				throw new InvalidOperationException();

			var rawData = _buffer.Slice(
				start: Position,
				length: sizeof(UInt16));

			var parsedData = BitConverter.ToUInt16(rawData);
			Position += sizeof(UInt16);
			return parsedData;
		}

		/// <summary>
		/// Reads 4 bytes from the stream and parses them as a UInt32.
		/// </summary>
		public UInt32 ReadUInt32() {
			if (BytesLeftToRead < sizeof(UInt32))
				throw new InvalidOperationException();

			var rawData = _buffer.Slice(
				start: Position,
				length: sizeof(UInt32));

			var parsedData = BitConverter.ToUInt32(rawData);
			Position += sizeof(UInt32);
			return parsedData;
		}

		/// <summary>
		/// Moves the stream forward <paramref name="byteCount"/> bytes.
		/// </summary>
		public void Skip(int byteCount = 1) {
			if (byteCount <= 0)
				throw new ArgumentOutOfRangeException(nameof(byteCount));
			if (BytesLeftToRead < byteCount)
				throw new ArgumentOutOfRangeException(nameof(byteCount));

			Position += byteCount;
		}
	}
}
