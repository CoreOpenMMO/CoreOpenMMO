using System;
using System.Text;

namespace COTS.GameServer.World.Loading {

    public struct WorldParsingStream {
        public readonly ByteArrayReadStream UnderlayingStream;

        public WorldParsingStream(ByteArrayReadStream rawStream) {
            if (rawStream == null)
                throw new ArgumentNullException(nameof(rawStream));

            UnderlayingStream = rawStream;
        }

        public bool IsOver {
            get {
                return UnderlayingStream.IsOver ||
                    (MarkupByte)UnderlayingStream.PeakByte() == MarkupByte.End;
            }
        }

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