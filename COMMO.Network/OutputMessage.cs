using System;

namespace COMMO.Network {

	public sealed class OutputMessage {
		public const int MaximumMessageSizeInBytes = 24590;

		private readonly byte[] _data = new byte[MaximumMessageSizeInBytes];

		public int Position { get; private set; } = 0;

		public void Clear() {
			Position = 0;
		}

		public bool CanWriteBytes(int byteCount) {
			if (byteCount < 0)
				throw new ArgumentNullException(nameof(byteCount));

			return MaximumMessageSizeInBytes - Position < byteCount;
		}

		public void AddByte(byte b) {
			if (!CanWriteBytes(sizeof(byte)))
				throw new InvalidOperationException();

			_data[Position] = b;
			Position += sizeof(byte);
		}

		public void AddBytes(ReadOnlySpan<byte> bytes) {
			if (!CanWriteBytes(bytes.Length))
				throw new InvalidOperationException();

			bytes.CopyTo(new Span<byte>(
				array: _data,
				start: Position,
				length: _data.Length - Position));
			Position += bytes.Length;
		}

		public void AddUInt16(UInt16 uint16) => AddBytes(BitConverter.GetBytes(uint16));

		public void AddUInt32(UInt32 uint32) => AddBytes(BitConverter.GetBytes(uint32));

		public ReadOnlySpan<byte> AsSpan() {
			return new ReadOnlySpan<byte>(
				array: _data,
				start: 0,
				length: Position);
		}
	}
}
