using System;
using System.Collections.Generic;
using System.Text;
using SharpServer.Server.Packets;

namespace SharpServer.Server.Utils
{
    public class ByteBuffer : IDisposable
    {
        List<byte> Buff;
        byte[] readBuff;
        int readpos;
        bool buffUpdate = false;

        public int Length2 { get; set; }

        public ByteBuffer()
        {
            Buff = new List<byte>();
            readpos = 6;
        }

        public void RSADecrypt()
        {
            Rsa.Decryp3(ref Buff, readpos);
        }

        public void SkipBytes(int count)
        {
            readpos += count;
        }

        public int GetReadPos()
        {
            return readpos;
        }

        public byte[] ToArray()
        {
            return Buff.ToArray();
        }

        public int Count()
        {
            return Buff.Count;
        }

        public int Length()
        {
            return Length2;
        }

        public void Clear()
        {
            Buff.Clear();
            readpos = 0;
        }
        #region"Write Data"
        public void WriteByte(byte Inputs)
        {
            Buff.Add(Inputs);
            buffUpdate = true;
        }

        public void WriteBytes(byte[] Input)
        {
            Buff.AddRange(Input);
            buffUpdate = true;
        }

        public void WriteShort(short Input)
        {
            Buff.AddRange(BitConverter.GetBytes(Input));
            buffUpdate = true;
        }

        public void WriteInteger(int Input)
        {
            Buff.AddRange(BitConverter.GetBytes(Input));
            buffUpdate = true;
        }

        public void WriteFloat(float Input)
        {
            Buff.AddRange(BitConverter.GetBytes(Input));
            buffUpdate = true;
        }

        public void WriteString(string Input)
        {
            try
            {
                Buff.AddRange(BitConverter.GetBytes(Input.Length));
                Buff.AddRange(Encoding.ASCII.GetBytes(Input));
                buffUpdate = true;
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region "Read Data"
        public string ReadString(bool Peek = true)
        {
            //int len = ReadInteger(true);
            if (buffUpdate)
            {
                readBuff = Buff.ToArray();
                buffUpdate = false;
            }

            string ret = Encoding.ASCII.GetString(readBuff, readpos, Length());
            if (Peek & Buff.Count > readpos)
            {
                if (ret.Length > 0)
                {
                    readpos += Length();
                }
            }
            return ret;
        }

        public byte ReadByte(bool Peek = true)
        {
            if (Buff.Count > readpos)
            {
                if (buffUpdate)
                {
                    readBuff = Buff.ToArray();
                    buffUpdate = false;
                }

                byte ret = readBuff[readpos];
                if (Peek & Buff.Count > readpos)
                {
                    readpos += 1;
                }
                return ret;
            }

            else
            {
                throw new Exception("Byte Buffer Past Limit!");
            }
        }

        public byte[] ReadBytes(int Length, bool Peek = true)
        {
            if (buffUpdate)
            {
                readBuff = Buff.ToArray();
                buffUpdate = false;
            }

            byte[] ret = Buff.GetRange(readpos, Length).ToArray();
            if (Peek)
            {
                readpos += Length;
            }
            return ret;
        }

        public float ReadFloat(bool Peek = true)
        {
            if (Buff.Count > readpos)
            {
                if (buffUpdate)
                {
                    readBuff = Buff.ToArray();
                    buffUpdate = false;
                }

                float ret = BitConverter.ToSingle(readBuff, readpos);
                if (Peek & Buff.Count > readpos)
                {
                    readpos += 4;
                }
                return ret;
            }

            else
            {
                throw new Exception("Byte Buffer is Past its Limit!");
            }
        }

        public int ReadInteger(bool Peek = true)
        {
            if (Buff.Count > readpos)
            {
                if (buffUpdate)
                {
                    readBuff = Buff.ToArray();
                    buffUpdate = false;
                }

                int ret = BitConverter.ToInt32(readBuff, readpos);
                if (Peek & Buff.Count > readpos)
                {
                    readpos += 4;
                }
                return ret;
            }

            else
            {
                throw new Exception("Byte Buffer is Past its Limit!");
            }
        }
        #endregion

        private bool disposedValue = false;

        //IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    Buff.Clear();
                }

                readpos = 0;
            }
            this.disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}