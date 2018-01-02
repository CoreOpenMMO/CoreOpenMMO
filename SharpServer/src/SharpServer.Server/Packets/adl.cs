using System;

namespace SharpServer.Server.Packets
{
    public class AdlerChecksum
    {
        #region Parameters
        /// <summary>
        /// AdlerBase is Adler-32 checksum algorithm parameter.
        /// </summary>
        public const uint AdlerBase = 0xFFF1;
        /// <summary>
        /// AdlerStart is Adler-32 checksum algorithm parameter.
        /// </summary>
        public const uint AdlerStart = 0x0001;
        /// <summary>
        /// AdlerBuff is Adler-32 checksum algorithm parameter.
        /// </summary>
        public const uint AdlerBuff = 0x0400;
        /// Adler-32 checksum value
        private uint m_unChecksumValue = 0;
        #endregion

        /// <value>
        /// ChecksumValue is property which enables the user
        /// to get Adler-32 checksum value for the last calculation 
        /// </value>
        public uint ChecksumValue
        {
            get { return m_unChecksumValue; }
        }

        /// <summary>
        /// Calculate Adler-32 checksum for buffer
        /// </summary>
        /// <param name="bytesBuff">Bites array for checksum calculation</param>
        /// <param name="unAdlerCheckSum">Checksum start value (default=1)</param>
        /// <returns>Returns true if the checksum values is successflly calculated</returns>
        public bool MakeForBuff(byte[] bytesBuff, uint unAdlerCheckSum)
        {
            m_unChecksumValue = Generate(ref bytesBuff, 0, bytesBuff.Length);
            return true;
        }

        /// <summary>
        /// Calculate Adler-32 checksum for buffer
        /// </summary>
        /// <param name="bytesBuff">Bites array for checksum calculation</param>
        /// <returns>Returns true if the checksum values is successflly calculated</returns>
        public bool MakeForBuff(byte[] bytesBuff)
        {
            return MakeForBuff(bytesBuff, AdlerStart);
        }

        /// Equals determines whether two files (buffers) 
        /// have the same checksum value (identical).
        /// </summary>
        /// <param name="obj">A AdlerChecksum object for comparison</param>
        /// <returns>Returns true if the value of checksum is the same
        /// as this instance; otherwise, false
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;

            AdlerChecksum other = (AdlerChecksum)obj;
            return (this.ChecksumValue == other.ChecksumValue);
        }

        /// <summary>
        /// operator== determines whether AdlerChecksum objects are equal.
        /// </summary>
        /// <param name="objA">A AdlerChecksum object for comparison</param>
        /// <param name="objB">A AdlerChecksum object for comparison</param>
        /// <returns>Returns true if the values of its operands are equal</returns>
        public static bool operator ==(AdlerChecksum objA, AdlerChecksum objB)
        {
            if (Object.Equals(objA, null) || Object.Equals(objB, null))
                return true;

            return objA.Equals(objB);
        }

        /// <summary>
        /// operator!= determines whether AdlerChecksum objects are not equal.
        /// </summary>
        /// <param name="objA">A AdlerChecksum object for comparison</param>
        /// <param name="objB">A AdlerChecksum object for comparison</param>
        /// <returns>Returns true if the values of its operands are not equal</returns>
        public static bool operator !=(AdlerChecksum objA, AdlerChecksum objB)
        {
            return !(objA == objB);
        }

        /// <summary>
        /// GetHashCode returns hash code for this instance.
        /// </summary>
        /// <returns>hash code of AdlerChecksum</returns>
        public override int GetHashCode()
        {
            return ChecksumValue.GetHashCode();
        }

        /// <summary>
        /// ToString is a method for current AdlerChecksum object
        /// representation in textual form.
        /// </summary>
        /// <returns>Returns current checksum or
        /// or "Unknown" if checksum value is unavailable 
        /// </returns>
        public override string ToString()
        {
            if (ChecksumValue != 0)
                return ChecksumValue.ToString();

            return "Unknown";
        }

        public static byte[] AddTo(byte[] packet)
        {
            byte[] fullPacket = new byte[packet.Length + 4];
            Array.Copy(packet, 2, fullPacket, 6, packet.Length - 2);
            Array.Copy(BitConverter.GetBytes((ushort)fullPacket.Length - 2), fullPacket, 2);
            Array.Copy(BitConverter.GetBytes(Generate(ref fullPacket, 6, fullPacket.Length)), 0, fullPacket, 2, 4);

            return fullPacket;
        }

        public static uint Generate(ref byte[] buffer, int index, int length)
        {
            if (buffer == null || length - index <= 0)
                return 0;

            uint unSum1 = AdlerStart & 0xFFFF;
            uint unSum2 = (AdlerStart >> 16) & 0xFFFF;

            for (int i = index; i < length; i++)
            {
                unSum1 = (unSum1 + buffer[i]) % AdlerBase;
                unSum2 = (unSum1 + unSum2) % AdlerBase;
            }

            return (unSum2 << 16) + unSum1;
        }
    }
}
