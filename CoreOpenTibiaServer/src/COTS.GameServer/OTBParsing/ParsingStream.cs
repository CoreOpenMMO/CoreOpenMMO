using System;
using System.Text;

namespace COTS.GameServer.OTBParsing {

	/// <remarks>
	/// !! This is a mutable struct !!
	/// Be careful when passing it to methods!
	/// </remarks>
	public struct ParsingStream {
		public readonly ByteArrayReadStream UnderlayingStream;
		public readonly int BeginPosition;
		public int CurrentPosition => UnderlayingStream.Position;
		public readonly int EndPosition;

		private byte[] _parsingBuffer;

		public ParsingStream(ParsingTree tree, ParsingNode node) {
			if (tree == null)
				throw new ArgumentNullException(nameof(tree));
			if (node == null)
				throw new ArgumentNullException(nameof(node));

			UnderlayingStream = new ByteArrayReadStream(
				array: tree.Data,
				position: node.DataBegin);

			BeginPosition = node.DataBegin;
			EndPosition = node.DataEnd;

			// The buffer must be at least as big as the largest non-string
			// object we can parse. Currently it's a uint32.
			_parsingBuffer = new byte[sizeof(UInt32)];
		}

		public bool IsOver => UnderlayingStream.IsOver || CurrentPosition >= EndPosition - 1;

		public byte ReadByte() {
			var value = UnderlayingStream.ReadByte();

			if ((MarkupByte)value != MarkupByte.Escape)
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
			for (var i = 0; i < byteCount; i++)
				ReadByte();
		}
	}
}