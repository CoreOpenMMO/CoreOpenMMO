using System;
using System.Collections.Generic;

namespace COMMO.IO
{
    public class ByteArrayMemoryStream : ByteArrayStream
    {
        private class Operation
        {
            public int Position { get; set; }

            public byte[] Buffer { get; set; }

            public int Offset { get; set; }

            public int Count { get; set; }
        }

        private readonly List<Operation> _operations = new List<Operation>();

		public override byte ReadByte() => throw new NotSupportedException();

		public override void Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

		public override void WriteByte(byte value) => Write(new byte[] { value }, 0, 1);

		public override void Write(byte[] buffer, int offset, int count)
        {
            _operations.Add(new Operation()
                {
                    Position = _position,

                    Buffer = buffer,

                    Offset = offset,

                    Count = count
                }
            );

            Seek(Origin.Current, count);
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[_length];

            var stream = new ByteArrayArrayStream(bytes);

            foreach (var operation in _operations)
            {
                stream.Seek(Origin.Begin, operation.Position);

                stream.Write(operation.Buffer, operation.Offset, operation.Count); 
            }

            return bytes;
        }
    }
}