using System;
using System.Text;

namespace COMMO.GameServer.OTBParsing {

	public ref struct OTBParsingStream {
		public readonly ReadOnlyMemoryStream UnderlayingStream;
		public int CurrentPosition => UnderlayingStream.Position;

		private byte[] _parsingBuffer;

		public OTBParsingStream(ReadOnlySpan<byte> otbData) {
			UnderlayingStream = new ReadOnlyMemoryStream(otbData);

			// The buffer must be at least as big as the largest non-string
			// object we can parse. Currently it's a uint32.
			_parsingBuffer = new byte[sizeof(UInt32)];
		}

		public bool IsOver => UnderlayingStream.IsOver;

		public byte ReadByte() {
			var value = UnderlayingStream.ReadByte();

			if ((OTBMarkupByte)value != OTBMarkupByte.Escape)
				return value;
			else
				return UnderlayingStream.ReadByte();
		}

		public UInt16 ReadUInt16() {
			for (var i = 0; i < sizeof(UInt16); i++)
				_parsingBuffer[i] = ReadByte();

			return BitConverter.ToUInt16(_parsingBuffer, 0);
		}

		public UInt32 ReadUInt32() {
			for (var i = 0; i < sizeof(UInt32); i++)
				_parsingBuffer[i] = ReadByte();

			return BitConverter.ToUInt32(_parsingBuffer, 0);
		}

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

		public void Skip(int byteCount = 1) {
			if (byteCount < 0)
				throw new ArgumentOutOfRangeException();

			for (var i = 0; i < byteCount; i++)
				ReadByte();
		}
	}
}
