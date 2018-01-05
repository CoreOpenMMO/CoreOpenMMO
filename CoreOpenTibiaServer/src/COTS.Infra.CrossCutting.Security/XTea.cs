using System;
using System.IO;

namespace COTS.Infra.CrossCutting.Security
{
    public static class XTea
    {
        /// <summary>
        /// Decryption method for Tibia XTea.
        /// Buffer based
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public unsafe static bool DecryptXtea(ref byte[] buffer, ref int length, int index, uint[] key)
        {
            if (length <= index || (length - index) % 8 > 0 || key == null)
                return false;

            fixed (byte* bufferPtr = buffer)
            {
                uint* words = (uint*)(bufferPtr + index);
                int msgSize = length - index;

                for (int pos = 0; pos < msgSize / 4; pos += 2)
                {
                    uint x_count = 32, x_sum = 0xC6EF3720, x_delta = 0x9E3779B9;

                    while (x_count-- > 0)
                    {
                        words[pos + 1] -= (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ x_sum
                            + key[x_sum >> 11 & 3];
                        x_sum -= x_delta;
                        words[pos] -= (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ x_sum
                            + key[x_sum & 3];
                    }
                }
            }

            length = (BitConverter.ToUInt16(buffer, index) + 2 + index);
            return true;
        }

        #region Conversion Needed!

        /// <summary>
        /// Based on Output network message
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public unsafe static bool EncryptXtea(OutputMessage msg, uint[] key)
        {
            if (key == null)
                return false;

            int pad = msg.Length % 8;
            if (pad > 0)
                msg.AddPaddingBytes(8 - pad);

            fixed (byte* bufferPtr = msg.Buffer)
            {
                uint* words = (uint*)(bufferPtr + msg.HeaderPosition);

                for (int pos = 0; pos < msg.Length / 4; pos += 2)
                {
                    uint x_sum = 0, x_delta = 0x9e3779b9, x_count = 32;

                    while (x_count-- > 0)
                    {
                        words[pos] += (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ x_sum
                            + key[x_sum & 3];
                        x_sum += x_delta;
                        words[pos + 1] += (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ x_sum
                            + key[x_sum >> 11 & 3];
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Same as method above for Decrypt, but uses NetworkMessage.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public unsafe static bool DecryptXtea(NetworkMessage msg, uint[] key)
        {
            if ((msg.Length - msg.Position) % 8 > 0 || key == null)
                return false;

            fixed (byte* bufferPtr = msg.Buffer)
            {
                uint* words = (uint*)(bufferPtr + msg.Position);
                int msgSize = msg.Length - msg.Position;

                for (int pos = 0; pos < msgSize / 4; pos += 2)
                {
                    uint x_count = 32, x_sum = 0xC6EF3720, x_delta = 0x9E3779B9;

                    while (x_count-- > 0)
                    {
                        words[pos + 1] -= (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ x_sum
                            + key[x_sum >> 11 & 3];
                        x_sum -= x_delta;
                        words[pos] -= (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ x_sum
                            + key[x_sum & 3];
                    }
                }
            }

            return true;
        }
        #endregion
    }


    /// <summary>
    /// Some constant-values.
    /// Same as for TFS 1.2/1
    /// </summary>
    public static class Constants
    {
        public const ushort NetworkMessageSizeMax = 24590;
        public const ushort NetworkMessageErrorSizeMax = NetworkMessageSizeMax - 16;

        public const int OutputMessagePoolSize = 100;
        public const int OutputMessagePoolExpansionSize = 10;

        public const int NetworkMessagePoolSize = 100;
        public const int NetworkMessagePoolExpansionSize = 10;
    }

    public static class Tools
    {
        public static uint AdlerChecksum(byte[] data, int index, int length)
        {
            const ushort adler = 65521;

            uint a = 1, b = 0;

            while (length > 0)
            {
                int tmp = (length > 5552) ? 5552 : length;
                length -= tmp;

                do
                {
                    a += data[index++];
                    b += a;
                } while (--tmp > 0);

                a %= adler;
                b %= adler;
            }

            return (b << 16) | a;
        }
    }

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
        /// Fills whole content of the given byte array to ByteStream 
        /// </summary>
        public NetworkMessage(byte[] buffer)
        {
            _length = buffer.Length;
            _position = 0;
            _buffer = buffer;
        }

        /// <summary>
        /// Fills whole content of the file to ByteStream 
        /// </summary>
        public NetworkMessage(FileStream fileStream)
        {
            _length = Convert.ToInt32(fileStream.Length);
            _position = 0;
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
            byte[] str = System.Text.Encoding.UTF8.GetBytes(value);
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

        public bool XteaDecrypt(uint[] key)
        {
            bool result = XTea.DecryptXtea(ref _buffer, ref _length, _position, key);

            ushort innerLength = GetUInt16();
            if (innerLength > Length - 8)
                result = false;

            Length = innerLength + _position;
            return result;
        }
    }


    public class OutputMessage : NetworkMessage
    {
        #region Constants
        public const int HeaderLength = 2;
        public const int CryptoLength = 4;
        public const int XteaMultiple = 8;
        public const int MaxBodyLength = Constants.NetworkMessageSizeMax - HeaderLength - CryptoLength - XteaMultiple;
        public const int MaxProtocolBodyLength = MaxBodyLength - 10;
        #endregion

        private int _headerPosition;
        /// <summary>
        /// Current position of header cursor 
        /// </summary>
        public int HeaderPosition
        {
            get { return _headerPosition; }
            protected set { _headerPosition = value; }
        }

        //public Connection MessageTarget; HERE SHOULD BE CONNECTION!
        public bool DisconnectAfterMessage;
        public bool IsRecycledMessage = true;

        public OutputMessage()
        {
            FreeMessage();
        }

        public void FreeMessage()
        {
            //allocate enough size for headers
            //2 bytes for unencrypted message size
            //4 bytes for checksum
            //2 bytes for encrypted message size
            _headerPosition = 8;
            //MessageTarget = null;
            DisconnectAfterMessage = false;

            Reset(8);
        }

        public void WriteMessageLength()
        {
            AddHeaderUInt16((ushort)Length);
        }

        public void AddCryptoHeader(bool addChecksum)
        {
            if (addChecksum)
            {
                AddHeaderUInt32(Tools.AdlerChecksum(Buffer, 6, Length));
            }

            AddHeaderUInt16((ushort)Length);
        }

        #region Add To Header
        protected void AddHeaderBytes(byte[] value)
        {
            if (value.Length > _headerPosition)
            {
                Console.WriteLine("OutputMessage AddHeader buffer is full!");
                return;
            }

            _headerPosition -= value.Length;
            Array.Copy(value, 0, Buffer, _headerPosition, value.Length);
            Length += value.Length;
        }

        protected void AddHeaderUInt32(uint value)
        {
            AddHeaderBytes(BitConverter.GetBytes(value));
        }

        protected void AddHeaderUInt16(ushort value)
        {
            AddHeaderBytes(BitConverter.GetBytes(value));
        }
        #endregion

        public void Append(NetworkMessage msg)
        {
            int msgLen = msg.Length;
            Buffer.MemCpy(Position, msg.Buffer, msgLen);
            Length += msgLen;
            Position += msgLen;
        }

        public void AddPaddingBytes(int count)
        {
            Buffer.MemSet(Position, (byte)ServerPacketType.PaddingByte, count);
            Length += count;
            Position += count;
        }
    }


    #region PacketTypes
    public enum ServerPacketType : byte
    {
        Disconnect = 0x0A,
        Disconnect1076 = 0x0B,
        DisconnectGame = 0x14,

        MOTD = 0x14,
        SessionKey = 0x28,
        CharacterList = 0x64,

        SelfAppear = 0x0A,
        GMAction = 0x0B,
        EnterWorld = 0x0F,
        ErrorMessage = 0x14,
        FyiMessage = 0x15,
        WaitingList = 0x16,

        Ping = 0x1E,
        WelcomeToGameServer = 0x1F,

        Death = 0x28,
        CanReportBugs = 0x32,

        PaddingByte = 0x33,

        MapDescription = 0x64,
        MapSliceNorth = 0x65,
        MapSliceEast = 0x66,
        MapSliceSouth = 0x67,
        MapSliceWest = 0x68,
        TileUpdate = 0x69,
        TileAddThing = 0x6A,
        TileUpdateThing = 0x6B,
        TileRemoveThing = 0x6C,
        CreatureMove = 0x6D,
        ContainerOpen = 0x6E,
        ContainerClose = 0x6F,
        ContainerAddItem = 0x70,
        ContainerUpdateItem = 0x71,
        ContainerRemoveItem = 0x72,
        InventorySetSlot = 0x78,
        InventoryClearSlot = 0x79,
        ShopWindowOpen = 0x7A,
        ShopSaleGoldCount = 0x7B,
        ShopWindowClose = 0x7C,
        SafeTradeRequestAck = 0x7D,
        SafeTradeRequestNoAck = 0x7E,
        SafeTradeClose = 0x7F,
        WorldLight = 0x82,
        MagicEffect = 0x83,
        AnimatedText = 0x84,
        Projectile = 0x85,
        CreatureSquare = 0x86,
        CreatureHealth = 0x8C,
        CreatureLight = 0x8D,
        CreatureOutfit = 0x8E,
        CreatureSpeed = 0x8F,
        CreatureSkull = 0x90,
        CreatureShield = 0x91,
        ItemTextWindow = 0x96,
        HouseTextWindow = 0x97,

        BasicData = 0x9F,

        PlayerStatus = 0xA0,
        PlayerSkillsUpdate = 0xA1,
        PlayerFlags = 0xA2,
        CancelTarget = 0xA3,

        SpellCooldown = 0xA4,
        SpellGroupCooldown = 0xA5,

        CreatureSpeech = 0xAA,
        ChannelList = 0xAB,
        ChannelOpen = 0xAC,
        ChannelOpenPrivate = 0xAD,
        RuleViolationOpen = 0xAE,
        RuleViolationRemove = 0xAF,
        RuleViolationCancel = 0xB0,
        RuleViolationLock = 0xB1,
        PrivateChannelCreate = 0xB2,
        ChannelClosePrivate = 0xB3,
        TextMessage = 0xB4,
        PlayerWalkCancel = 0xB5,
        FloorChangeUp = 0xBE,
        FloorChangeDown = 0xBF,
        OutfitWindow = 0xC8,
        VipState = 0xD2,
        VipLogin = 0xD3,
        VipLogout = 0xD4,
        QuestList = 0xF0,
        QuestPartList = 0xF1,
        ShowTutorial = 0xDC,
        AddMapMarker = 0xDD,

        ChatChannelEvent = 0xF3,

    }

    public enum ClientPacketType : byte
    {
        LoginServerRequest = 0x01,
        GameServerRequest = 0x0A,
        Disconnect = 0x0F,

        Logout = 0x14,
        PingBack = 0x1D,
        Ping = 0x1E,

        AutoWalk = 0x64,
        MoveNorth = 0x65,
        MoveEast = 0x66,
        MoveSouth = 0x67,
        MoveWest = 0x68,
        StopAutoWalk = 0x69,
        MoveNorthEast = 0x6A,
        MoveSouthEast = 0x6B,
        MoveSouthWest = 0x6C,
        MoveNorthWest = 0x6D,
        TurnNorth = 0x6F,

        TurnEast = 0x70,
        TurnSouth = 0x71,
        TurnWest = 0x72,
        ItemMove = 0x78,
        ShopOpen = 0x79,
        ShopBuy = 0x7A,
        ShopSell = 0x7B,
        ShopClose = 0x7C,
        TradeRequest = 0x7D,
        TradeLook = 0x7E,
        TradeAccept = 0x7F,

        TradeClose = 0x80,
        ItemUse = 0x82,
        ItemUseOn = 0x83,
        ItemUseBattlelist = 0x84,
        ItemRotate = 0x85,
        ContainerClose = 0x87,
        ContainerOpenParent = 0x88,
        TextWindow = 0x89,
        HouseWindow = 0x8A,
        LookAt = 0x8C,
        LookInBattleWindow = 0x8D,
        JoinAggression = 0x8E,

        PlayerSpeech = 0x96,
        ChannelList = 0x97,
        ChannelOpen = 0x98,
        ChannelClose = 0x99,
        PrivateChannelOpen = 0x9A,
        NpcChannelClose = 0x9E,

        FightModes = 0xA0,
        Attack = 0xA1,
        Follow = 0xA2,
        PartyInvite = 0xA3,
        PartyJoin = 0xA4,
        PartyInviteRevoke = 0xA5,
        PartyPassLeadership = 0xA6,
        PartyLeave = 0xA7,
        PartySharedExperienceEnable = 0xA8,
        PrivateChannelCreate = 0xAA,
        PrivateChannelInvite = 0xAB,
        PrivateChannelExclude = 0xAC,

        CancelAttackAndFollow = 0xBE,

        UpdateTile = 0xC9,
        UpdateContainer = 0xCA,
        BrowseField = 0xCB,
        SeekInContainer = 0xCC,

        RequestOutfit = 0xD2,
        ChangeOutfit = 0xD3,
        ToggleMount = 0xD4,
        VipAdd = 0xDC,
        VipRemove = 0xDD,
        VipEdit = 0xDE,

        BugReport = 0xE6,
        ThankYou = 0xE7,
        DebugAssert = 0xE8,

        QuestShowLog = 0xF0,
        QuestLine = 0xF1,
        RuleViolationReport = 0xF2,
        GetObjectInfo = 0xF3,
        MarketLeave = 0xF4,
        MarketBrowse = 0xF5,
        MarketCreateOffer = 0xF6,
        MarketCancelOffer = 0xF7,
        MarketAcceptOffer = 0xF8,
        ModalWindowAnswer = 0xF9,
    }
    #endregion


}
