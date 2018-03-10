using System;

namespace COMMO {

	public ref struct ReadOnlyMemoryStream {
		private readonly ReadOnlySpan<Byte> _buffer;
		public int Position { get; private set; }

		// This field is used while BitConverter doesn't support spans
		private readonly byte[] _parsingBuffer;

		public ReadOnlyMemoryStream(ReadOnlySpan<Byte> buffer, int position = 0) {
			if (position < 0 || position > buffer.Length)
				throw new ArgumentOutOfRangeException(nameof(position));

			_buffer = buffer;
			Position = position;
			_parsingBuffer = new byte[sizeof(UInt32)];
		}

		public bool IsOver => Position >= _buffer.Length;

		public int BytesLeftToRead() => _buffer.Length - Position;

		public byte PeakByte() => _buffer[Position];

		public byte ReadByte() {
			if (IsOver)
				throw new InvalidOperationException();

			var data = _buffer[Position];
			Position += sizeof(byte);
			return data;
		}

		public UInt16 ReadUInt16() {
			if (Position + sizeof(UInt16) > _buffer.Length)
				throw new InvalidOperationException();

			for(int i=0; i < sizeof(UInt16); i++) {
				_parsingBuffer[i] = _buffer[Position];
				Position++;
			}

			return BitConverter.ToUInt16(_parsingBuffer, 0);
		}

		public UInt32 ReadUInt32() {
			if (Position + sizeof(UInt32) > _buffer.Length)
				throw new InvalidOperationException();

			for (int i = 0; i < sizeof(UInt32); i++) {
				_parsingBuffer[i] = _buffer[Position];
				Position++;
			}

			return BitConverter.ToUInt32(_parsingBuffer, 0);
		}

		public void Skip(int byteCount = 1) {
			if (byteCount <= 0)
				throw new ArgumentOutOfRangeException(nameof(byteCount));
			if (Position + byteCount > _buffer.Length)
				throw new ArgumentOutOfRangeException(nameof(byteCount));

			Position += byteCount;
		}
	}
}
