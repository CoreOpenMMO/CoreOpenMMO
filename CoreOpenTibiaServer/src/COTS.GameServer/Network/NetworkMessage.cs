using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace COTS.GameServer.Network
{
    //public class NetworkMessage : IDisposable
    //{
    //    private int _readPos;
    //    private readonly int _initialPos = 6;
    //    private readonly int _maxBufferSize = 24590;

    //    public byte[] Buffer { get; set; }
    //    public Socket WorkSocket { get; set; }
    //    public StringBuilder StringBuilder = new StringBuilder();

    //    public NetworkMessage()
    //    {
    //        Buffer = new byte[_maxBufferSize];
    //        _readPos = _initialPos;
    //    }

    //    public void SkipBytes(int count)
    //    {
    //        _readPos += count;
    //    }

    //    public int GetReadPos()
    //    {
    //        return _readPos;
    //    }

    //    public byte[] ToArray()
    //    {
    //        return Buffer.ToArray();
    //    }

    //    public int Count()
    //    {
    //        return Buffer.ToList().Count;
    //    }

    //    public int Length()
    //    {
    //        return Buffer.Length;
    //    }

    //    public void Clear()
    //    {
    //        Buffer = new byte[_maxBufferSize];
    //        _readPos = _initialPos;
    //    }

    //    #region"Write Data"
    //    public void WriteByte(byte inputs)
    //    {
    //        Buffer.ToList().Add(inputs);
    //    }

    //    public void WriteBytes(byte[] input)
    //    {
    //        Buffer.ToList().AddRange(input);
    //    }

    //    public void WriteShort(short input)
    //    {
    //        Buffer.ToList().AddRange(BitConverter.GetBytes(input));
    //    }

    //    public void WriteInteger(int input)
    //    {
    //        Buffer.ToList().AddRange(BitConverter.GetBytes(input));
    //    }

    //    public void WriteFloat(float input)
    //    {
    //        Buffer.ToList().AddRange(BitConverter.GetBytes(input));
    //    }

    //    public void WriteString(string input)
    //    {
    //        try
    //        {
    //            Buffer.ToList().AddRange(BitConverter.GetBytes(input.Length));
    //            Buffer.ToList().AddRange(Encoding.ASCII.GetBytes(input));
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine(ex.Message);
    //        }
    //    }
    //    #endregion

    //    #region "Read Data"

    //    public byte ReadByte(bool peek = true)
    //    {
    //        if (Buffer.ToList().Count > _readPos)
    //        {
    //            var ret = Buffer[_readPos];
    //            if (peek & Buffer.ToList().Count > _readPos)
    //                _readPos += 1;

    //            return ret;
    //        }

    //        throw new Exception("Byte Buffer Past Limit!");
    //    }

    //    public byte[] ReadBytes(int length, bool peek = true)
    //    {
    //        var ret = Buffer.ToList().GetRange(_readPos, length).ToArray();
    //        if (peek)
    //            _readPos += length;

    //        return ret;
    //    }

    //    public float ReadFloat(bool peek = true)
    //    {
    //        if (Buffer.ToList().Count > _readPos)
    //        {
    //            var ret = BitConverter.ToSingle(Buffer, _readPos);
    //            if (peek & Buffer.ToList().Count > _readPos)
    //                _readPos += 4;

    //            return ret;
    //        }

    //        throw new Exception("Byte Buffer is Past its Limit!");
    //    }

    //    public int ReadInteger(bool peek = true)
    //    {
    //        if (Buffer.ToList().Count > _readPos)
    //        {
    //            var ret = BitConverter.ToInt32(Buffer, _readPos);
    //            if (peek & Buffer.ToList().Count > _readPos)
    //                _readPos += 4;

    //            return ret;
    //        }

    //        throw new Exception("Byte Buffer is Past its Limit!");
    //    }
    //    #endregion

    //    public byte[] ReadBytes(int count)
    //    {
    //        if (_readPos + count > Length())
    //            throw new IndexOutOfRangeException("NetworkMessage GetBytes() out of range.");

    //        var result = new byte[count];
    //        Array.Copy(Buffer, _readPos, result, 0, count);
    //        _readPos += count;
    //        return result;
    //    }

    //    public string ReadString()
    //    {
    //        var len = ReadUInt16();
    //        var result = Encoding.Default.GetString(Buffer, _readPos, len);
    //        _readPos += len;
    //        return result;
    //    }

    //    public ushort ReadUInt16()
    //    {
    //        return BitConverter.ToUInt16(ReadBytes(2), 0);
    //    }

    //    private bool _disposedValue = false;

    //    //IDisposable
    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (!_disposedValue)
    //        {
    //            if (disposing)
    //                Buffer.ToList().Clear();

    //            _readPos = _initialPos;
    //        }
    //        _disposedValue = true;
    //    }

    //    public void Dispose()
    //    {
    //        Dispose(true);
    //        GC.SuppressFinalize(this);
    //    }
    //}
}
