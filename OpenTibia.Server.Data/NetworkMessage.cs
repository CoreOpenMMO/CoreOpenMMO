// <copyright file="NetworkMessage.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Data
{
    using System;
    using System.Text;
    using OpenTibia.Security.Encryption;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    public class NetworkMessage
    {
        private byte[] buffer;
        private readonly int bufferSize = 16394;
        private int length;
        private int position;

        public int Length => this.length;

        public int Position => this.position;

        public byte[] Buffer => this.buffer;

        public int BufferSize => this.bufferSize;

        public NetworkMessage()
        {
            this.Reset();
        }

        public NetworkMessage(int startingIndex)
        {
            this.Reset(startingIndex);
        }

        public void Reset(int startingIndex)
        {
            this.buffer = new byte[this.bufferSize];
            this.length = startingIndex;
            this.position = startingIndex;
        }

        public void Reset()
        {
            this.Reset(2);
        }

        public byte GetByte()
        {
            if (this.Position + 1 > this.Length)
            {
                throw new IndexOutOfRangeException("NetworkMessage GetByte() out of range.");
            }

            return this.buffer[this.position++];
        }

        public byte[] GetBytes(int count)
        {
            if (this.Position + count > this.Length)
            {
                throw new IndexOutOfRangeException("NetworkMessage GetBytes() out of range.");
            }

            byte[] t = new byte[count];
            Array.Copy(this.buffer, this.Position, t, 0, count);

            this.position += count;
            return t;
        }

        public static NetworkMessage Copy(NetworkMessage inMessage)
        {
            if (inMessage == null)
            {
                throw new ArgumentNullException(nameof(inMessage));
            }

            NetworkMessage newMessage = new NetworkMessage();

            inMessage.Buffer.CopyTo(newMessage.buffer, 0);

            newMessage.length = inMessage.Length;
            newMessage.position = inMessage.Position;

            return newMessage;
        }

        public string GetString()
        {
            int len = this.GetUInt16();
            string t = ASCIIEncoding.Default.GetString(this.buffer, this.Position, len);

            this.position += len;
            return t;
        }

        public ushort GetUInt16()
        {
            return BitConverter.ToUInt16(this.GetBytes(2), 0);
        }

        public uint GetUInt32()
        {
            return BitConverter.ToUInt32(this.GetBytes(4), 0);
        }

        public byte[] GetPacket()
        {
            byte[] t = new byte[this.Length - 2];
            Array.Copy(this.buffer, 2, t, 0, this.Length - 2);
            return t;
        }

        private ushort GetPacketHeader()
        {
            return BitConverter.ToUInt16(this.buffer, 0);
        }

        public void AddPacket(IPacketOutgoing packet)
        {
            packet.Add(this);
        }

        public void AddByte(byte value)
        {
            if (1 + this.Length > this.bufferSize)
            {
                throw new Exception("NetworkMessage buffer is full.");
            }

            this.AddBytes(new[] { value });
        }

        public void AddBytes(byte[] value)
        {
            if (value.Length + this.Length > this.bufferSize)
            {
                throw new Exception("NetworkMessage buffer is full.");
            }

            Array.Copy(value, 0, this.buffer, this.Position, value.Length);

            this.position += value.Length;

            if (this.Position > this.Length)
            {
                this.length = this.Position;
            }
        }

        public void AddString(string value)
        {
            this.AddUInt16((ushort)value.Length);
            this.AddBytes(ASCIIEncoding.Default.GetBytes(value));
        }

        public void AddUInt16(ushort value)
        {
            this.AddBytes(BitConverter.GetBytes(value));
        }

        public void AddUInt32(uint value)
        {
            this.AddBytes(BitConverter.GetBytes(value));
        }

        public void AddPaddingBytes(int count)
        {
            this.position += count;

            if (this.Position > this.Length)
            {
                this.length = this.Position;
            }
        }

        public void AddLocation(Location location)
        {
            this.AddUInt16((ushort)location.X);
            this.AddUInt16((ushort)location.Y);
            this.AddByte((byte)location.Z);
        }

        public void AddCreature(ICreature creature, bool asKnown, uint creatureToRemoveId)
        {
            if (asKnown)
            {
                this.AddByte((byte)GameOutgoingPacketType.AddKnownCreature); // known
                this.AddByte(0x00);
                this.AddUInt32(creature.CreatureId);
            }
            else
            {
                this.AddByte((byte)GameOutgoingPacketType.AddUnknownCreature); // unknown
                this.AddByte(0x00);
                this.AddUInt32(creatureToRemoveId);
                this.AddUInt32(creature.CreatureId);
                this.AddString(creature.Name);
            }

            this.AddByte(Convert.ToByte(Math.Min(100, creature.Hitpoints * 100 / creature.MaxHitpoints))); // health bar, needs a percentage.
            this.AddByte(Convert.ToByte(creature.ClientSafeDirection));

            if (creature.IsInvisible)
            {
                this.AddUInt16(0x00);
                this.AddUInt16(0x00);
            }
            else
            {
                this.AddOutfit(creature.Outfit);
            }

            this.AddByte(creature.LightBrightness);
            this.AddByte(creature.LightColor);
            this.AddUInt16(creature.Speed);

            this.AddByte(creature.Skull);
            this.AddByte(creature.Shield);
        }

        private void AddOutfit(Outfit outfit)
        {
            // Add creature outfit
            this.AddUInt16(outfit.Id);

            if (outfit.Id > 0)
            {
                this.AddByte(outfit.Head);
                this.AddByte(outfit.Body);
                this.AddByte(outfit.Legs);
                this.AddByte(outfit.Feet);
            }
            else
            {
                this.AddUInt16(outfit.LikeType);
            }
        }

        public void AddItem(IItem item)
        {
            if (item == null)
            {
                // TODO: log this.
                Console.WriteLine("Add: Null item passed.");
                return;
            }

            this.AddUInt16(item.Type.ClientId);

            if (item.IsCumulative)
            {
                this.AddByte(item.Amount);
            }
            else if (item.IsLiquidPool || item.IsLiquidSource || item.IsLiquidContainer)
            {
                this.AddByte(item.LiquidType);
            }
        }

        public byte PeekByte()
        {
            return this.buffer[this.Position];
        }

        public void Resize(int size)
        {
            this.length = size;
            this.position = 0;
        }

        public byte[] PeekBytes(int count)
        {
            byte[] t = new byte[count];
            Array.Copy(this.buffer, this.Position, t, 0, count);
            return t;
        }

        public ushort PeekUInt16()
        {
            return BitConverter.ToUInt16(this.PeekBytes(2), 0);
        }

        public uint PeekUInt32()
        {
            return BitConverter.ToUInt32(this.PeekBytes(4), 0);
        }

        public string PeekString()
        {
            int len = this.PeekUInt16();
            return ASCIIEncoding.ASCII.GetString(this.PeekBytes(len + 2), 2, len);
        }

        public void ReplaceBytes(int index, byte[] value)
        {
            if (this.Length - index >= value.Length)
            {
                Array.Copy(value, 0, this.buffer, index, value.Length);
            }
        }

        public void SkipBytes(int count)
        {
            if (this.Position + count > this.Length)
            {
                throw new IndexOutOfRangeException("NetworkMessage SkipBytes() out of range.");
            }

            this.position += count;
        }

        public void RsaDecrypt(bool useCipKeys = true)
        {
            Rsa.Decrypt(ref this.buffer, this.position, this.length, useCipKeys);
        }

        public bool XteaDecrypt(uint[] key)
        {
            return Xtea.Decrypt(ref this.buffer, ref this.length, 2, key);
        }

        public bool XteaEncrypt(uint[] key)
        {
            return Xtea.Encrypt(ref this.buffer, ref this.length, 2, key);
        }

        private void InsertPacketLength()
        {
            Array.Copy(BitConverter.GetBytes((ushort)(this.length - 4)), 0, this.buffer, 2, 2);
        }

        private void InsertTotalLength()
        {
            Array.Copy(BitConverter.GetBytes((ushort)(this.length - 2)), 0, this.buffer, 0, 2);
        }

        public bool PrepareToSendWithoutEncryption(bool insertOnlyOneLength = false)
        {
            if (!insertOnlyOneLength)
            {
                this.InsertPacketLength();
            }

            this.InsertTotalLength();

            return true;
        }

        public bool PrepareToSend(uint[] xteaKey)
        {
            // Must be before Xtea, because the packet length is encrypted as well
            this.InsertPacketLength();

            if (!this.XteaEncrypt(xteaKey))
            {
                return false;
            }

            // Must be after Xtea, because takes the checksum of the encrypted packet
            // InsertAdler32();
            this.InsertTotalLength();

            return true;
        }

        public bool PrepareToRead(uint[] xteaKey)
        {
            if (!this.XteaDecrypt(xteaKey))
            {
                return false;
            }

            this.position = 4;
            return true;
        }
    }
}
