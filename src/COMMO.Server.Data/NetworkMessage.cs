// <copyright file="NetworkMessage.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;
using COMMO.Security.Encryption;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;

namespace COMMO.Server.Data
{
    public class NetworkMessage
    {
        private byte[] buffer;
        private readonly int bufferSize = 16394;
        private int length;
        private int position;

        public int Length => length;

        public int Position => position;

        public byte[] Buffer => buffer;

        public int BufferSize => bufferSize;

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
            buffer = new byte[bufferSize];
            length = startingIndex;
            position = startingIndex;
        }

        public void Reset()
        {
            Reset(2);
        }

        public byte GetByte()
        {
            if (Position + 1 > Length)
            {
                throw new IndexOutOfRangeException("NetworkMessage GetByte() out of range.");
            }

            return buffer[position++];
        }

        public byte[] GetBytes(int count)
        {
            if (Position + count > Length)
            {
                throw new IndexOutOfRangeException("NetworkMessage GetBytes() out of range.");
            }

            byte[] t = new byte[count];
            Array.Copy(buffer, Position, t, 0, count);

            position += count;
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
            int len = GetUInt16();
            string t = ASCIIEncoding.Default.GetString(buffer, Position, len);

            position += len;
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

        public byte[] GetPacket()
        {
            byte[] t = new byte[Length - 2];
            Array.Copy(buffer, 2, t, 0, Length - 2);
            return t;
        }

        private ushort GetPacketHeader()
        {
            return BitConverter.ToUInt16(buffer, 0);
        }

        public void AddPacket(IPacketOutgoing packet)
        {
            packet.Add(this);
        }

        public void AddByte(byte value)
        {
            if (1 + Length > bufferSize)
            {
                throw new Exception("NetworkMessage buffer is full.");
            }

            AddBytes(new[] { value });
        }

        public void AddBytes(byte[] value)
        {
            if (value.Length + Length > bufferSize)
            {
                throw new Exception("NetworkMessage buffer is full.");
            }

            Array.Copy(value, 0, buffer, Position, value.Length);

            position += value.Length;

            if (Position > Length)
            {
                length = Position;
            }
        }

        public void AddString(string value)
        {
            AddUInt16((ushort)value.Length);
            AddBytes(ASCIIEncoding.Default.GetBytes(value));
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
            position += count;

            if (Position > Length)
            {
                length = Position;
            }
        }

        public void AddLocation(Location location)
        {
            AddUInt16((ushort)location.X);
            AddUInt16((ushort)location.Y);
            AddByte((byte)location.Z);
        }

        public void AddCreature(ICreature creature, bool asKnown, uint creatureToRemoveId)
        {
            if (asKnown)
            {
                AddByte((byte)GameOutgoingPacketType.AddKnownCreature); // known
                AddByte(0x00);
                AddUInt32(creature.CreatureId);
            }
            else
            {
                AddByte((byte)GameOutgoingPacketType.AddUnknownCreature); // unknown
                AddByte(0x00);
                AddUInt32(creatureToRemoveId);
                AddUInt32(creature.CreatureId);
                AddString(creature.Name);
            }

            AddByte(Convert.ToByte(Math.Min(100, creature.Hitpoints * 100 / creature.MaxHitpoints))); // health bar, needs a percentage.
            AddByte(Convert.ToByte(creature.ClientSafeDirection));

            if (creature.IsInvisible)
            {
                AddUInt16(0x00);
                AddUInt16(0x00);
            }
            else
            {
                AddOutfit(creature.Outfit);
            }

            AddByte(creature.LightBrightness);
            AddByte(creature.LightColor);
            AddUInt16(creature.Speed);

            AddByte(creature.Skull);
            AddByte(creature.Shield);
        }

        private void AddOutfit(Outfit outfit)
        {
            // Add creature outfit
            AddUInt16(outfit.Id);

            if (outfit.Id > 0)
            {
                AddByte(outfit.Head);
                AddByte(outfit.Body);
                AddByte(outfit.Legs);
                AddByte(outfit.Feet);
            }
            else
            {
                AddUInt16(outfit.LikeType);
            }
        }

        public void AddItem(IItem item)
        {
            if (item == null)
            {
                // TODO: log 
                Console.WriteLine("Add: Null item passed.");
                return;
            }

            AddUInt16(item.Type.ClientId);

            if (item.IsCumulative)
            {
                AddByte(item.Amount);
            }
            else if (item.IsLiquidPool || item.IsLiquidSource || item.IsLiquidContainer)
            {
                AddByte(item.LiquidType);
            }
        }

        public byte PeekByte()
        {
            return buffer[Position];
        }

        public void Resize(int size)
        {
            length = size;
            position = 0;
        }

        public byte[] PeekBytes(int count)
        {
            byte[] t = new byte[count];
            Array.Copy(buffer, Position, t, 0, count);
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
            int len = PeekUInt16();
            return ASCIIEncoding.ASCII.GetString(PeekBytes(len + 2), 2, len);
        }

        public void ReplaceBytes(int index, byte[] value)
        {
            if (Length - index >= value.Length)
            {
                Array.Copy(value, 0, buffer, index, value.Length);
            }
        }

        public void SkipBytes(int count)
        {
            if (Position + count > Length)
            {
                throw new IndexOutOfRangeException("NetworkMessage SkipBytes() out of range.");
            }

            position += count;
        }

        public void RsaDecrypt(bool useCipKeys = true)
        {
            Rsa.Decrypt(ref buffer, position, length, useCipKeys);
        }

        public bool XteaDecrypt(uint[] key)
        {
            return Xtea.Decrypt(ref buffer, ref length, 2, key);
        }

        public bool XteaEncrypt(uint[] key)
        {
            return Xtea.Encrypt(ref buffer, ref length, 2, key);
        }

        private void InsertPacketLength()
        {
            Array.Copy(BitConverter.GetBytes((ushort)(length - 4)), 0, buffer, 2, 2);
        }

        private void InsertTotalLength()
        {
            Array.Copy(BitConverter.GetBytes((ushort)(length - 2)), 0, buffer, 0, 2);
        }

        public bool PrepareToSendWithoutEncryption(bool insertOnlyOneLength = false)
        {
            if (!insertOnlyOneLength)
            {
                InsertPacketLength();
            }

            InsertTotalLength();

            return true;
        }

        public bool PrepareToSend(uint[] xteaKey)
        {
            // Must be before Xtea, because the packet length is encrypted as well
            InsertPacketLength();

            if (!XteaEncrypt(xteaKey))
            {
                return false;
            }

            // Must be after Xtea, because takes the checksum of the encrypted packet
            // InsertAdler32();
            InsertTotalLength();

            return true;
        }

        public bool PrepareToRead(uint[] xteaKey)
        {
            if (!XteaDecrypt(xteaKey))
            {
                return false;
            }

            position = 4;
            return true;
        }
    }
}
