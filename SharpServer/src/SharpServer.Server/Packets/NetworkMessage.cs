using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace SharpServer.Server.Packets
{
    public class NetworkMessage
    {
        #region Instance Variables

        private int _position;
        public const int BufferSize = 16394;
        public byte[] Buffer = new byte[BufferSize];
        public Socket WorkSocket = null;
        public StringBuilder StringBuilder = new StringBuilder();

        private int _length;

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }
        
        #endregion
        
        #region Properties

        public byte[] GetPacket()
        {
            byte[] t = new byte[Length - 8];
            Array.Copy(Buffer, 8, t, 0, Length - 8);
            return t;
        }

        #endregion

        #region Constructors

        public NetworkMessage()
        {
            Reset();
        }

        public NetworkMessage(int startingIndex)
        {
            Reset(startingIndex);
        }

        public void Reset(int startingIndex)
        {
            Buffer = new byte[BufferSize];
            Length = startingIndex;
            _position = startingIndex;
        }

        public void Reset()
        {
            Length = Buffer.Length;
            Reset(8);
        }

        #endregion

        #region Get

        public byte GetByte()
        {
            if (_position + 1 > Length)
                throw new IndexOutOfRangeException("NetworkMessage GetByte() out of range.");

            return Buffer[_position++];
        }

        public byte[] GetBytes(int count)
        {
            if (_position + count > Length)
                throw new IndexOutOfRangeException("NetworkMessage GetBytes() out of range.");

            byte[] t = new byte[count];
            Array.Copy(Buffer, _position, t, 0, count);
            _position += count;
            return t;
        }

        public string GetString()
        {
            int len = (int)GetUInt16();
            string t = System.Text.ASCIIEncoding.Default.GetString(Buffer, _position, len);
            _position += len;
            return t;
        }

        public ushort GetUInt16()
        {
            return BitConverter.ToUInt16(GetBytes(2), 0);
        }

        public uint GetUInt32()
        {
            return BitConverter.ToUInt32(GetBytes(4), 0);
        }

       

        private ushort GetPacketHeader()
        {
            return BitConverter.ToUInt16(Buffer, 0);
        }

        
        #endregion

        #region Add

        public void AddByte(byte value)
        {
            if (1 + Length > BufferSize)
                throw new Exception("NetworkMessage buffer is full.");

            AddBytes(new byte[] { value });
        }

        public void AddBytes(byte[] value)
        {
            if (value.Length + Length > BufferSize)
                throw new Exception("NetworkMessage buffer is full.");

            Array.Copy(value, 0, Buffer, _position, value.Length);
            _position += value.Length;

            if (_position > Length)
                Length = _position;
        }

        public void AddString(string value)
        {
            AddUInt16((ushort)value.Length);
            AddBytes(System.Text.ASCIIEncoding.Default.GetBytes(value));
        }

        public void AddUInt16(ushort value)
        {
            AddBytes(BitConverter.GetBytes(value));
        }

        public void AddUInt32(uint value)
        {
            AddBytes(BitConverter.GetBytes(value));
        }
        
        public void AddPaddingBytes(int count)
        {
            _position += count;

            if (_position > Length)
                Length = _position;
        }

        #endregion

        #region Peek

        public byte PeekByte()
        {
            return Buffer[_position];
        }

        public byte[] PeekBytes(int count)
        {
            byte[] t = new byte[count];
            Array.Copy(Buffer, _position, t, 0, count);
            return t;
        }

        public ushort PeekUInt16()
        {
            return BitConverter.ToUInt16(PeekBytes(2), 0);
        }

        public uint PeekUInt32()
        {
            return BitConverter.ToUInt32(PeekBytes(4), 0);
        }

        public string PeekString()
        {
            int len = (int)PeekUInt16();
            return System.Text.ASCIIEncoding.ASCII.GetString(PeekBytes(len + 2), 2, len);
        }

        #endregion

        #region Replace

        public void ReplaceBytes(int index, byte[] value)
        {
            if (Length - index >= value.Length)
                Array.Copy(value, 0, Buffer, index, value.Length);
        }

        #endregion

        #region Skip

        public void SkipBytes(int count)
        {
            if (_position + count > Length)
                throw new IndexOutOfRangeException("NetworkMessage SkipBytes() out of range.");
            _position += count;
        }

        #endregion

        #region Encryption

        public void RSADecrypt()
        {
            
            //Rsa.RSADecrypt(ref Buffer, _position);
        }

        public bool XteaDecrypt(uint[] key)
        {
            return Xtea.Decrypt(ref Buffer, ref _length, 6, key);
        }

        public bool XteaEncrypt(uint[] key)
        {
            return Xtea.Encrypt(ref Buffer, ref _length, 6, key);
        }

        #endregion

        #region Checksum

        public bool CheckAdler32()
        {
            return AdlerChecksum.Generate(ref Buffer, 6, Length) == GetAdler32();
        }

        public void InsertAdler32()
        {
            Array.Copy(BitConverter.GetBytes(AdlerChecksum.Generate(ref Buffer, 6, Length)), 0, Buffer, 2, 4);
        }

        private uint GetAdler32()
        {
            return BitConverter.ToUInt32(Buffer, 2);
        }

        #endregion

        #region Prepare

        private void InsertPacketLength()
        {
            Array.Copy(BitConverter.GetBytes((ushort)(Length - 8)), 0, Buffer, 6, 2);
        }

        private void InsertTotalLength()
        {
            Array.Copy(BitConverter.GetBytes((ushort)(Length - 2)), 0, Buffer, 0, 2);
        }

        public bool PrepareToSendWithoutEncryption()
        {
            InsertPacketLength();

            InsertAdler32();
            InsertTotalLength();

            return true;
        }

        public bool PrepareToSend(uint[] xteaKey)
        {
            // Must be before Xtea, because the packet length is encrypted as well
            InsertPacketLength();

            if (!XteaEncrypt(xteaKey))
                return false;

            // Must be after Xtea, because takes the checksum of the encrypted packet
            InsertAdler32();
            InsertTotalLength();

            return true;
        }

        public bool PrepareToRead(uint[] xteaKey)
        {
            if (!XteaDecrypt(xteaKey))
                return false;

            _position = 8;
            return true;
        }

        #endregion
    }
}
