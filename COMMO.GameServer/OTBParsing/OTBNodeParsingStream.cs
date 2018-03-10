using System;
using System.Text;

namespace COMMO.GameServer.OTBParsing {

	/// <summary>
	/// This struct is used to parse a OTBNode.
	/// To understand why it require both a reference to the node and to the tree which 
	/// the node belongs to, <see cref="OTBTree"/>.
	/// </summary>
	/// <remarks>
	/// !! This is a mutable struct !!
	/// Be careful when passing it to methods!
	/// </remarks>
	public struct OTBNodeParsingStream {
		public readonly ByteArrayReadStream UnderlayingStream;
		public readonly int BeginPosition;
		public int CurrentPosition => UnderlayingStream.Position;
		public readonly int EndPosition;

		private byte[] _parsingBuffer;

		public OTBNodeParsingStream(OTBTree tree, OTBNode nodeToParse) {
			if (tree == null)
				throw new ArgumentNullException(nameof(tree));
			if (nodeToParse == null)
				throw new ArgumentNullException(nameof(nodeToParse));

			UnderlayingStream = new ByteArrayReadStream(
				array: tree.Data,
				position: nodeToParse.DataBegin);

			BeginPosition = nodeToParse.DataBegin;
			EndPosition = nodeToParse.DataEnd;

			// The buffer must be at least as big as the largest non-string
			// object we can parse. Currently it's a uint32.
			_parsingBuffer = new byte[sizeof(UInt32)];
		}

		public bool IsOver => UnderlayingStream.IsOver || CurrentPosition >= EndPosition - 1;

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
			for (var i = 0; i < byteCount; i++)
				ReadByte();
		}
	}
}