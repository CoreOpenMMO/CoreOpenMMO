using System;
using System.Net.Sockets;

namespace COTS.Infra.CrossCutting.Security
{
    public class NetworkMessage
    {
        #region Properties
        private int _length;
        /// <summary>
        /// Length of the current content
        /// </summary>
        public int Length
        {
            get { return _length; }
            protected set { _length = value; }
        }

        private int _position;
        /// <summary>
        /// Current position of cursor 
        /// </summary>
        public int Position
        {
            get { return _position; }
            protected set { _position = value; }
        }

        private byte[] _buffer;
        /// <summary>
        /// Content holder
        /// </summary>
        public byte[] Buffer
        {
            get { return _buffer; }
            protected set { _buffer = value; }
        }

        private uint[] _key;

        public uint[] Key
        {
            get { return _key; }
            set { _key = value; }
        }


        private Socket _handler;

        public Socket Handler
        {
            get { return _handler; }
            protected set { _handler = value; }
        }
        
        /// <summary>
        /// Maximum buffer size of empty instance
        /// </summary>
        private const int BufferSize = Constants.NetworkMessageSizeMax;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates empty ByteStream 
        /// </summary>
        public NetworkMessage()
        {
            _length = 0;
            _position = 0;
            _buffer = new byte[Constants.NetworkMessageSizeMax];
        }

        /// <summary>
        /// Creates empty ByteStream 
        /// </summary>
        public NetworkMessage(Socket handler)
        {
            _length = 0;
            _position = 0;
            _buffer = new byte[Constants.NetworkMessageSizeMax];
            _handler = handler;
        }

        /// <summary>
        /// Fills whole content of the given byte array to ByteStream 
        /// </summary>
        public NetworkMessage(byte[] buffer, Socket handler)
        {
            _length = buffer.Length;
            _position = 6;
            _buffer = buffer;
            _handler = handler;
            _key = new uint[4];
        }

        /// <summary>
        /// Fills whole content of the file to ByteStream 
        /// </summary>
        public NetworkMessage(NetworkStream fileStream)
        {
            _length = Convert.ToInt32(fileStream.Length);
            _position = 6;
            _buffer = new byte[_length];

            fileStream.Read(_buffer, 0, _length);
        }
        #endregion

        #region Utility
        /// <summary>
        /// Resets network message's position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="length"></param>
        public void Reset(int position = 0, int length = 0)
        {
            _length = length;
            _position = position;
        }

        /// <summary>
        /// Convert the content of ByteStream into an byte array
        /// </summary>
        /// <returns>Byte array containing ByteStream data</returns>
        public byte[] ToArray()
        {
            byte[] t = new byte[Length];
            Array.Copy(_buffer, 0, t, 0, Length);
            return t;
        }

        /// <summary>
        /// Jumps specified count of bytes   
        /// </summary>
        public void SkipBytes(int count)
        {
            _position += count;

            if (_position > _length)
                _length = _position;
        }
        #endregion

        #region Get
        /// <summary>
        /// Returns the next byte from the content 
        /// </summary>
        public byte GetByte()
        {
            if (_position + 1 > _length)
                throw new IndexOutOfRangeException("NetworkMessage GetByte() out of range.");

            return _buffer[_position++];
        }

        /// <summary>
        /// Returns the next byte from the content 
        /// </summary>
        public byte GetPreviousByte()
        {
            if (_position - 1 < 0)
                throw new IndexOutOfRangeException("NetworkMessage GetByte() out of range.");

            return _buffer[--_position];
        }

        /// <summary>
        /// Returns next specified count of bytes from the content
        /// </summary>
        /// <param name="count">Byte count that will be returned</param>
        public byte[] GetBytes(int count)
        {
            if (_position + count > _length)
                throw new IndexOutOfRangeException("NetworkMessage GetBytes() out of range.");

            byte[] t = new byte[count];
            Array.Copy(_buffer, _position, t, 0, count);
            _position += count;
            return t;
        }

        /// <summary>
        /// Returns next string from the content - next first 2 bytes will be considered as string length  
        /// </summary>
        public string GetString()
        {
            int len = GetUInt16();
            string t = System.Text.Encoding.UTF8.GetString(_buffer, _position, len);
            _position += len;
            return t;
        }

        /// <summary>
        /// Returns next 2 bytes as unsigned short integer  
        /// </summary>
        public ushort GetUInt16()
        {
            return BitConverter.ToUInt16(GetBytes(2), 0);
        }

        /// <summary>
        /// Returns next 4 bytes as unsigned integer  
        /// </summary>
        public uint GetUInt32()
        {
            return BitConverter.ToUInt32(GetBytes(4), 0);
        }

        /// <summary>
        /// Returns next 2 bytes as short integer  
        /// </summary>
        public short GetInt16()
        {
            return BitConverter.ToInt16(GetBytes(2), 0);
        }

        /// <summary>
        /// Returns next 4 bytes as integer  
        /// </summary>
        public int GetInt32()
        {
            return BitConverter.ToInt32(GetBytes(4), 0);
        }

        /// <summary>
        /// Returns next 8 bytes as long integer  
        /// </summary>
        public long GetInt64()
        {
            return BitConverter.ToInt64(GetBytes(8), 0);
        }

        /// <summary>
        /// Returns next 8 bytes as double  
        /// </summary>
        public double GetDouble()
        {
            return BitConverter.ToDouble(GetBytes(8), 0);
        }

        /// <summary>
        /// Returns next byte as boolean  
        /// </summary>
        public bool GetBoolean()
        {
            return BitConverter.ToBoolean(GetBytes(1), 0);
        }

        /// <summary>
        /// Returns next 8 bytes as DateTime  
        /// </summary>
        public DateTime GetDateTime()
        {
            return DateTime.FromBinary(BitConverter.ToInt64(GetBytes(8), 0));
        }

        /// <summary>
        /// Returns next string from the content - next first byte is boolean next 2 bytes will be considered as string length 
        /// </summary>
        public string GetNullableString()
        {
            if (!GetBoolean())
                return null;
            int len = GetUInt16();
            string t = System.Text.Encoding.UTF8.GetString(_buffer, _position, len);
            _position += len;
            return t;
        }

        /// <summary>
        /// Returns next 3 bytes as nullable unsigned short integer  
        /// </summary>
        public ushort? GetNullableUInt16()
        {
            if (!GetBoolean())
                return null;
            return BitConverter.ToUInt16(GetBytes(2), 0);
        }

        /// <summary>
        /// Returns next 5 bytes as nullable unsigned integer  
        /// </summary>
        public uint? GetNullableUInt32()
        {
            if (!GetBoolean())
                return null;
            return BitConverter.ToUInt32(GetBytes(4), 0);
        }

        /// <summary>
        /// Returns next 3 bytes as nullable short integer  
        /// </summary>
        public short? GetNullableInt16()
        {
            if (!GetBoolean())
                return null;
            return BitConverter.ToInt16(GetBytes(2), 0);
        }

        /// <summary>
        /// Returns next 5 bytes as nullable integer  
        /// </summary>
        public int? GetNullableInt32()
        {
            if (!GetBoolean())
                return null;
            return BitConverter.ToInt32(GetBytes(4), 0);
        }

        /// <summary>
        /// Returns next 9 bytes as null long integer  
        /// </summary>
        public long? GetNullableInt64()
        {
            if (!GetBoolean())
                return null;
            return BitConverter.ToInt64(GetBytes(8), 0);
        }

        /// <summary>
        /// Returns next 9 bytes as nullable double  
        /// </summary>
        public double? GetNullableDouble()
        {
            if (!GetBoolean())
                return null;
            return BitConverter.ToDouble(GetBytes(8), 0);
        }

        /// <summary>
        /// Returns next 2 bytes as nullable boolean  
        /// </summary>
        public bool? GetNullableBoolean()
        {
            if (!GetBoolean())
                return null;
            return BitConverter.ToBoolean(GetBytes(1), 0);
        }

        /// <summary>
        /// Returns next 9 bytes as nullable DateTime  
        /// </summary>
        public DateTime? GetNullableDateTime()
        {
            if (!GetBoolean())
                return null;
            return DateTime.FromBinary(BitConverter.ToInt64(GetBytes(8), 0));
        }

        #endregion

        #region Add
        /// <summary>
        /// Adds a byte to the content  
        /// </summary>
        public void AddByte(byte value)
        {
            if (1 + _length > BufferSize)
                throw new Exception("NetworkMessage buffer is full.");

            AddBytes(new[] { value });
        }

        /// <summary>
        /// Adds an array of bytes to the content  
        /// </summary>
        public void AddBytes(byte[] value)
        {
            if (value.Length + _length > BufferSize)
                throw new Exception("NetworkMessage buffer is full.");

            Array.Copy(value, 0, _buffer, _position, value.Length);
            _position += value.Length;

            if (_position > _length)
                _length += value.Length;

            //if (_position > _length)
            //    _length = _position;
        }

        /// <summary>
        /// Adds a string to the content  
        /// </summary>
        public void AddString(string value)
        {
            byte[] str = System.Text.Encoding.ASCII.GetBytes(value);
            AddUInt16((ushort)str.Length);
            AddBytes(str);
        }

        /// <summary>
        /// Adds an unsigned short integer to the content  
        /// </summary>
        public void AddUInt16(ushort value)
        {
            AddBytes(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Adds an unsigned integer to the content  
        /// </summary>
        public void AddUInt32(uint value)
        {
            AddBytes(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Adds an unsigned integer to the content  
        /// </summary>
        public void AddUInt64(ulong value)
        {
            AddBytes(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Adds a short integer to the content  
        /// </summary>
        public void AddInt16(short value)
        {
            AddBytes(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Adds an integer to the content  
        /// </summary>
        public void AddInt32(int value)
        {
            AddBytes(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Adds a long integer to the content  
        /// </summary>
        public void AddInt64(long value)
        {
            AddBytes(BitConverter.GetBytes(value));
        }

        ///// <summary>
        ///// Adds a double to the content  
        ///// </summary>
        //public void AddDouble(double value)
        //{
        //    AddBytes(BitConverter.GetBytes(value));
        //}

        /// <summary>
        /// Adds a double to the content  
        /// </summary>
        public void AddDouble(double value, byte precision = 2)
        {
            AddByte(precision);
            AddUInt32((uint)(value * Math.Pow(10, precision)) + int.MaxValue);
        }

        /// <summary>
        /// Adds a boolean to the content  
        /// </summary>
        public void AddBoolean(bool value)
        {
            AddBytes(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Adds a DateTime to the content  
        /// </summary>
        public void AddDateTime(DateTime value)
        {
            AddBytes(BitConverter.GetBytes(value.Ticks));
        }

        /// <summary>
        /// Adds a nullable string to the content  
        /// </summary>
        public void AddNullableString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                AddBytes(BitConverter.GetBytes(false));
                return;
            }
            AddBytes(BitConverter.GetBytes(true));
            byte[] str = System.Text.Encoding.UTF8.GetBytes(value);
            AddUInt16((ushort)str.Length);
            AddBytes(str);
        }

        /// <summary>
        /// Adds a nullable unsigned short integer to the content  
        /// </summary>
        public void AddNullableUInt16(ushort? value)
        {
            if (!value.HasValue)
            {
                AddBytes(BitConverter.GetBytes(false));
                return;
            }
            AddBytes(BitConverter.GetBytes(true));
            AddBytes(BitConverter.GetBytes(value.Value));
        }

        /// <summary>
        /// Adds a nullable unsigned integer to the content  
        /// </summary>
        public void AddNullableUInt32(uint? value)
        {
            if (!value.HasValue)
            {
                AddBytes(BitConverter.GetBytes(false));
                return;
            }
            AddBytes(BitConverter.GetBytes(true));
            AddBytes(BitConverter.GetBytes(value.Value));
        }

        /// <summary>
        /// Adds a nullable short integer to the content  
        /// </summary>
        public void AddNullableInt16(short? value)
        {
            if (!value.HasValue)
            {
                AddBytes(BitConverter.GetBytes(false));
                return;
            }
            AddBytes(BitConverter.GetBytes(true));
            AddBytes(BitConverter.GetBytes(value.Value));
        }

        /// <summary>
        /// Adds a nullable integer to the content  
        /// </summary>
        public void AddNullableInt32(int? value)
        {
            if (!value.HasValue)
            {
                AddBytes(BitConverter.GetBytes(false));
                return;
            }
            AddBytes(BitConverter.GetBytes(true));
            AddBytes(BitConverter.GetBytes(value.Value));
        }

        /// <summary>
        /// Adds a nullable long integer to the content  
        /// </summary>
        public void AddNullableInt64(long? value)
        {
            if (!value.HasValue)
            {
                AddBytes(BitConverter.GetBytes(false));
                return;
            }
            AddBytes(BitConverter.GetBytes(true));
            AddBytes(BitConverter.GetBytes(value.Value));
        }

        /// <summary>
        /// Adds a nullable double to the content  
        /// </summary>
        public void AddNullableDouble(double? value)
        {
            if (!value.HasValue)
            {
                AddBytes(BitConverter.GetBytes(false));
                return;
            }
            AddBytes(BitConverter.GetBytes(true));
            AddBytes(BitConverter.GetBytes(value.Value));
        }

        /// <summary>
        /// Adds a nullable boolean to the content  
        /// </summary>
        public void AddNullableBoolean(bool? value)
        {
            if (!value.HasValue)
            {
                AddBytes(BitConverter.GetBytes(false));
                return;
            }
            AddBytes(BitConverter.GetBytes(true));
            AddBytes(BitConverter.GetBytes(value.Value));
        }

        /// <summary>
        /// Adds a nullable DateTime to the content  
        /// </summary>
        public void AddNullableDateTime(DateTime? value)
        {
            if (!value.HasValue)
            {
                AddBytes(BitConverter.GetBytes(false));
                return;
            }
            AddBytes(BitConverter.GetBytes(true));
            AddBytes(BitConverter.GetBytes(value.Value.Ticks));
        }
        #endregion

        public bool RsaDecrypt()
        {
            if (_length - _position < 128)
                return false;

            Rsa.Decrypt(ref _buffer, _position);
            return GetByte() == 0;
        }

        public bool XteaDecrypt()
        {
            _key = new uint[4];

            bool result = XTea.DecryptXtea(ref _buffer, ref _length, _position, _key);

            ushort innerLength = GetUInt16();
            if (innerLength > Length - 8)
                result = false;

            Length = innerLength + _position;
            return result;
        }
    }
}