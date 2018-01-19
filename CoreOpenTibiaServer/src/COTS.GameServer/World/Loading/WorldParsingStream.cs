using System;
using System.Text;

namespace COTS.GameServer.World.Loading {

    /// <remarks>
    /// !! This is a mutable struct !!
    /// Be careful when passing it to methods!
    /// </remarks>
    public struct WorldParsingStream {
        public readonly ByteArrayReadStream UnderlayingStream;
        public readonly int BeginPosition;
        public int CurrentPosition => UnderlayingStream.Position;
        public readonly int EndPosition;

        public WorldParsingStream(ParsingTree tree, ParsingNode node) {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            this.UnderlayingStream = new ByteArrayReadStream(
                array: tree.Data,
                position: node.PropsBegin);

            this.BeginPosition = node.PropsBegin;
            this.EndPosition = node.PropsEnd;
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
            var serializedValue = new byte[sizeof(UInt16)];
            for (int i = 0; i < serializedValue.Length; i++)
                serializedValue[i] = this.ReadByte();

            return BitConverter.ToUInt16(serializedValue, 0);
        }

        public UInt32 ReadUInt32() {
            var serializedValue = new byte[sizeof(UInt32)];
            for (int i = 0; i < serializedValue.Length; i++)
                serializedValue[i] = this.ReadByte();

            return BitConverter.ToUInt32(serializedValue, 0);
        }

        public string ReadString() {
            var stringLength = ReadUInt16();
            var stringData = new byte[stringLength];

            for (int i = 0; i < stringLength; i++)
                stringData[i] = ReadByte();

            // When in C land, use C encoding...
            return Encoding.ASCII.GetString(stringData);
        }
    }
}