namespace COMMO.OTB {
	using COMMO.Utilities;
	using System;
	using System.Text;

	/// <summary>
	/// This class is used to parse .otb files.
	/// </summary>
	public ref struct OTBParsingStream {
		public readonly ReadOnlyMemoryStream UnderlayingStream;
		public int CurrentPosition => UnderlayingStream.Position;

		private byte[] _parsingBuffer;

		/// <summary>
		/// Creates a new instace of <see cref="OTBParsingStream"/>.
		/// </summary>
		public OTBParsingStream(ReadOnlySpan<byte> otbData) {
			UnderlayingStream = new ReadOnlyMemoryStream(otbData);

			// The buffer must be at least as big as the largest non-string
			// object we can parse. Currently it's a UInt32.
			_parsingBuffer = new byte[sizeof(UInt32)];
		}

		/// <summary>
		/// Returns true if there are no bytes left to read.
		/// Returns false otherwise.
		/// </summary>
		public bool IsOver => UnderlayingStream.IsOver;

		/// <summary>
		/// Reads a byte from the underlaying stream, considering OTB's escape values.
		/// </summary>
		public byte ReadByte() {
			var value = UnderlayingStream.ReadByte();

			if ((OTBMarkupByte)value != OTBMarkupByte.Escape)
				return value;
			else
				return UnderlayingStream.ReadByte();
		}

		/// <summary>
		/// Reads a bytes from the underlaying stream, considering OTB's escape values,
		/// until enough bytes were read to parse them as a UInt16.
		/// </summary>
		public UInt16 ReadUInt16() {
			for (var i = 0; i < sizeof(UInt16); i++)
				_parsingBuffer[i] = ReadByte();

			return BitConverter.ToUInt16(_parsingBuffer, 0);
		}

		/// <summary>
		/// Reads a bytes from the underlaying stream, considering OTB's escape values,
		/// until enough bytes were read to parse them as a UInt32.
		/// </summary>
		public UInt32 ReadUInt32() {
			for (var i = 0; i < sizeof(UInt32); i++)
				_parsingBuffer[i] = ReadByte();

			return BitConverter.ToUInt32(_parsingBuffer, 0);
		}

		/// <summary>
		/// Reads a bytes from the underlaying stream, considering OTB's escape values,
		/// until enough bytes were read to parse them as a ASCII-encoded string.
		/// The first 2 bytes read (considering OTB's escape values) represent the string length.
		/// </summary>
		public string ReadString() {
			var stringLength = ReadUInt16();

			// "Resize" our buffer, iff necessary
			if (stringLength > _parsingBuffer.Length)
				_parsingBuffer = new byte[stringLength];

			for (var i = 0; i < stringLength; i++)
				_parsingBuffer[i] = ReadByte();

			// When in C land, use C encoding...
			return Encoding.ASCII.GetString(_parsingBuffer);
		}

		/// <summary>
		/// Skips <paramref name="byteCount"/> bytes from the underlaying stream, considering OTB's escape values.
		/// </summary>
		public void Skip(int byteCount = 1) {
			if (byteCount <= 0)
				throw new ArgumentOutOfRangeException();

			for (var i = 0; i < byteCount; i++)
				ReadByte();
		}
	}
}
