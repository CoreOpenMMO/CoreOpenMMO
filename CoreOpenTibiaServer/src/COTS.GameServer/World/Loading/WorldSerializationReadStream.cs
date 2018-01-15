using System;

namespace COTS.GameServer.World.Loading {

    public sealed class WorldSerializationReadStream {
        public readonly ByteArrayReadStream UnderlayingStream;

        public WorldSerializationReadStream(ByteArrayReadStream underlayingStream) {
            if (underlayingStream == null)
                throw new ArgumentNullException(nameof(underlayingStream));

            UnderlayingStream = underlayingStream;
        }

        public byte ReadByte() {
            var value = UnderlayingStream.ReadByte();

            if ((ReservedByte)value != ReservedByte.Escape)
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
    }
}