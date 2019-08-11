using System;
using System.IO;

namespace COMMO.IO
{
    public abstract class ByteArrayBufferedStream : ByteArrayStream, IDisposable
    {
        private readonly Stream _stream;
        
        public ByteArrayBufferedStream(Stream stream)
        {
            _stream = stream;
        }

        ~ByteArrayBufferedStream()
        {
            Dispose(false);
        }

        private readonly byte[] _bytes = new byte[4 * 1024];

        private int _bytesPosition;

        private int _bytesLength;
        
        private void Load()
        {
            _stream.Seek(_bytesPosition = _position, SeekOrigin.Begin); _bytesLength = _stream.Read(_bytes, 0, _bytes.Length);
        }
        
        public byte GetByte()
        {
            int index = _position - _bytesPosition;

            if (index < 0 || _bytesLength - index < 1)
            {
                Load();

                index = 0;
            }

            byte value = _bytes[index];

            Seek(Origin.Current, 1);

            return value;
        }

        public void GetBytes(byte[] buffer, int offset, int count)
        {
            int index = _position - _bytesPosition;

            if (index < 0 || _bytesLength - index < count)
            {
                Load();

                index = 0;
            }

            Buffer.BlockCopy(_bytes, index, buffer, offset, count);

            Seek(Origin.Current, count);
        }

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    if (_stream != null)
                    {
                        _stream.Dispose();
                    }
                }
            }
        }
    }
}