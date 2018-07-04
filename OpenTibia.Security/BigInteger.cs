// <copyright file="BigInteger.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Security
{
    using System;

    /**
        * Copyright (c) 2002 Chew Keong TAN
        * All rights reserved
        *
        * http://www.codeproject.com/KB/cs/biginteger.aspx
        *
        * Permission is hereby granted, free of charge, to any person obtaining
        * a copy of this software and associated documentation files (the "Software"),
        * to deal in the Software without restriction, including without limitation
        * the rights to use, copy, modify, merge, publish, distribute, and/or sell
        * copies of the Software, and to permit persons to whom the Software is
        * furnished to do so, provided that the above copyright notice(s) and this
        * permission notice appear in all copies of the Software and that both the
        * above copyright notice(s) and this permission notice appear in supporting
        * documentation.
        *
        * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT OF THIRD PARTY RIGHTS.
        * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN THIS NOTICE BE
        * LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR CONSEQUENTIAL DAMAGES, OR
        * ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER
        * IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT
        * OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
        * */
    public class BigInteger
    {
        // maximum length of the BigInteger in uint (4 bytes)
        // change this to suit the required level of precision.
        private const int MaxLength = 70;

        private readonly uint[] data;             // stores bytes from the Big Integer
        public int DataLength;                 // number of actual chars used

        // ***********************************************************************
        // Constructor (Default value for BigInteger is 0
        // ***********************************************************************
        public BigInteger()
        {
            this.data = new uint[MaxLength];
            this.DataLength = 1;
        }

        // ***********************************************************************
        // Constructor (Default value provided by long)
        // ***********************************************************************
        public BigInteger(long value)
        {
            this.data = new uint[MaxLength];
            long tempVal = value;

            // copy bytes from long to BigInteger without any assumption of
            // the length of the long datatype
            this.DataLength = 0;
            while (value != 0 && this.DataLength < MaxLength)
            {
                this.data[this.DataLength] = (uint)(value & 0xFFFFFFFF);
                value >>= 32;
                this.DataLength++;
            }

            if (tempVal > 0) // overflow check for +ve value
            {
                if (value != 0 || (this.data[MaxLength - 1] & 0x80000000) != 0)
                {
                    throw new ArithmeticException("Positive overflow in constructor.");
                }
            }
            else if (tempVal < 0) // underflow check for -ve value
            {
                if (value != -1 || (this.data[this.DataLength - 1] & 0x80000000) == 0)
                {
                    throw new ArithmeticException("Negative underflow in constructor.");
                }
            }

            if (this.DataLength == 0)
            {
                this.DataLength = 1;
            }
        }

        // ***********************************************************************
        // Constructor (Default value provided by ulong)
        // ***********************************************************************
        public BigInteger(ulong value)
        {
            this.data = new uint[MaxLength];

            // copy bytes from ulong to BigInteger without any assumption of
            // the length of the ulong datatype
            this.DataLength = 0;
            while (value != 0 && this.DataLength < MaxLength)
            {
                this.data[this.DataLength] = (uint)(value & 0xFFFFFFFF);
                value >>= 32;
                this.DataLength++;
            }

            if (value != 0 || (this.data[MaxLength - 1] & 0x80000000) != 0)
            {
                throw new ArithmeticException("Positive overflow in constructor.");
            }

            if (this.DataLength == 0)
            {
                this.DataLength = 1;
            }
        }

        // ***********************************************************************
        // Constructor (Default value provided by BigInteger)
        // ***********************************************************************
        public BigInteger(BigInteger bi)
        {
            this.data = new uint[MaxLength];

            this.DataLength = bi.DataLength;

            for (int i = 0; i < this.DataLength; i++)
            {
                this.data[i] = bi.data[i];
            }
        }

        // ***********************************************************************
        // Constructor (Default value provided by a string of digits of the
        //              specified base)
        //
        // Example (base 10)
        // -----------------
        // To initialize "a" with the default value of 1234 in base 10
        //      BigInteger a = new BigInteger("1234", 10)
        //
        // To initialize "a" with the default value of -1234
        //      BigInteger a = new BigInteger("-1234", 10)
        //
        // Example (base 16)
        // -----------------
        // To initialize "a" with the default value of 0x1D4F in base 16
        //      BigInteger a = new BigInteger("1D4F", 16)
        //
        // To initialize "a" with the default value of -0x1D4F
        //      BigInteger a = new BigInteger("-1D4F", 16)
        //
        // Note that string values are specified in the <sign><magnitude>
        // format.
        //
        // ***********************************************************************
        public BigInteger(string value, int radix)
        {
            BigInteger multiplier = new BigInteger(1);
            BigInteger result = new BigInteger();
            value = value.ToUpper().Trim();
            int limit = 0;

            if (value[0] == '-')
            {
                limit = 1;
            }

            for (int i = value.Length - 1; i >= limit; i--)
            {
                int posVal = value[i];

                if (posVal >= '0' && posVal <= '9')
                {
                    posVal -= '0';
                }
                else if (posVal >= 'A' && posVal <= 'Z')
                {
                    posVal = (posVal - 'A') + 10;
                }
                else
                {
                    posVal = 9999999;       // arbitrary large
                }

                if (posVal >= radix)
                {
                    throw new ArithmeticException("Invalid string in constructor.");
                }

                if (value[0] == '-')
                {
                    posVal = -posVal;
                }

                result = result + (multiplier * posVal);

                if ((i - 1) >= limit)
                {
                    multiplier = multiplier * radix;
                }
            }

            if (value[0] == '-') // negative values
            {
                if ((result.data[MaxLength - 1] & 0x80000000) == 0)
                {
                    throw new ArithmeticException("Negative underflow in constructor.");
                }
            }
            else // positive values
            {
                if ((result.data[MaxLength - 1] & 0x80000000) != 0)
                {
                    throw new ArithmeticException("Positive overflow in constructor.");
                }
            }

            this.data = new uint[MaxLength];
            for (int i = 0; i < result.DataLength; i++)
            {
                this.data[i] = result.data[i];
            }

            this.DataLength = result.DataLength;
        }

        // ***********************************************************************
        // Constructor (Default value provided by an array of bytes)
        //
        // The lowest index of the input byte array (i.e [0]) should contain the
        // most significant byte of the number, and the highest index should
        // contain the least significant byte.
        //
        // E.g.
        // To initialize "a" with the default value of 0x1D4F in base 16
        //      byte[] temp = { 0x1D, 0x4F };
        //      BigInteger a = new BigInteger(temp)
        //
        // Note that this method of initialization does not allow the
        // sign to be specified.
        //
        // ***********************************************************************
        public BigInteger(byte[] inData)
        {
            this.DataLength = inData.Length >> 2;

            int leftOver = inData.Length & 0x3;
            if (leftOver != 0) // length not multiples of 4
            {
                this.DataLength++;
            }

            if (this.DataLength > MaxLength)
            {
                throw new ArithmeticException("Byte overflow in constructor.");
            }

            this.data = new uint[MaxLength];

            for (int i = inData.Length - 1, j = 0; i >= 3; i -= 4, j++)
            {
                this.data[j] = (uint)((inData[i - 3] << 24) + (inData[i - 2] << 16) +
                                 (inData[i - 1] << 8) + inData[i]);
            }

            if (leftOver == 1)
            {
                this.data[this.DataLength - 1] = inData[0];
            }
            else if (leftOver == 2)
            {
                this.data[this.DataLength - 1] = (uint)((inData[0] << 8) + inData[1]);
            }
            else if (leftOver == 3)
            {
                this.data[this.DataLength - 1] = (uint)((inData[0] << 16) + (inData[1] << 8) + inData[2]);
            }

            while (this.DataLength > 1 && this.data[this.DataLength - 1] == 0)
            {
                this.DataLength--;
            }

            // Console.WriteLine("Len = " + dataLength);
        }

        // ***********************************************************************
        // Constructor (Default value provided by an array of bytes of the
        // specified length.)
        // ***********************************************************************
        public BigInteger(byte[] inData, int inLen)
        {
            this.DataLength = inLen >> 2;

            int leftOver = inLen & 0x3;
            if (leftOver != 0) // length not multiples of 4
            {
                this.DataLength++;
            }

            if (this.DataLength > MaxLength || inLen > inData.Length)
            {
                throw new ArithmeticException("Byte overflow in constructor.");
            }

            this.data = new uint[MaxLength];

            for (int i = inLen - 1, j = 0; i >= 3; i -= 4, j++)
            {
                this.data[j] = (uint)((inData[i - 3] << 24) + (inData[i - 2] << 16) +
                                 (inData[i - 1] << 8) + inData[i]);
            }

            if (leftOver == 1)
            {
                this.data[this.DataLength - 1] = inData[0];
            }
            else if (leftOver == 2)
            {
                this.data[this.DataLength - 1] = (uint)((inData[0] << 8) + inData[1]);
            }
            else if (leftOver == 3)
            {
                this.data[this.DataLength - 1] = (uint)((inData[0] << 16) + (inData[1] << 8) + inData[2]);
            }

            if (this.DataLength == 0)
            {
                this.DataLength = 1;
            }

            while (this.DataLength > 1 && this.data[this.DataLength - 1] == 0)
            {
                this.DataLength--;
            }

            // Console.WriteLine("Len = " + dataLength);
        }

        // ***********************************************************************
        // Constructor (Default value provided by an array of unsigned integers)
        // *********************************************************************
        public BigInteger(uint[] inData)
        {
            this.DataLength = inData.Length;

            if (this.DataLength > MaxLength)
            {
                throw new ArithmeticException("Byte overflow in constructor.");
            }

            this.data = new uint[MaxLength];

            for (int i = this.DataLength - 1, j = 0; i >= 0; i--, j++)
            {
                this.data[j] = inData[i];
            }

            while (this.DataLength > 1 && this.data[this.DataLength - 1] == 0)
            {
                this.DataLength--;
            }

            // Console.WriteLine("Len = " + dataLength);
        }

        // ***********************************************************************
        // Overloading of the typecast operator.
        // For BigInteger bi = 10;
        // ***********************************************************************
        public static implicit operator BigInteger(long value)
        {
            return new BigInteger(value);
        }

        public static implicit operator BigInteger(ulong value)
        {
            return new BigInteger(value);
        }

        public static implicit operator BigInteger(int value)
        {
            return new BigInteger(value);
        }

        public static implicit operator BigInteger(uint value)
        {
            return new BigInteger((ulong)value);
        }

        // ***********************************************************************
        // Overloading of addition operator
        // ***********************************************************************
        public static BigInteger operator +(BigInteger bi1, BigInteger bi2)
        {
            BigInteger result = new BigInteger();

            result.DataLength = (bi1.DataLength > bi2.DataLength) ? bi1.DataLength : bi2.DataLength;

            long carry = 0;
            for (int i = 0; i < result.DataLength; i++)
            {
                long sum = bi1.data[i] + (long)bi2.data[i] + carry;
                carry = sum >> 32;
                result.data[i] = (uint)(sum & 0xFFFFFFFF);
            }

            if (carry != 0 && result.DataLength < MaxLength)
            {
                result.data[result.DataLength] = (uint)carry;
                result.DataLength++;
            }

            while (result.DataLength > 1 && result.data[result.DataLength - 1] == 0)
            {
                result.DataLength--;
            }

            // overflow check
            int lastPos = MaxLength - 1;
            if ((bi1.data[lastPos] & 0x80000000) == (bi2.data[lastPos] & 0x80000000) &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw new ArithmeticException();
            }

            return result;
        }

        // ***********************************************************************
        // Overloading of the unary ++ operator
        // ***********************************************************************
        public static BigInteger operator ++(BigInteger bi1)
        {
            BigInteger result = new BigInteger(bi1);

            long val, carry = 1;
            int index = 0;

            while (carry != 0 && index < MaxLength)
            {
                val = result.data[index];
                val++;

                result.data[index] = (uint)(val & 0xFFFFFFFF);
                carry = val >> 32;

                index++;
            }

            if (index > result.DataLength)
            {
                result.DataLength = index;
            }
            else
            {
                while (result.DataLength > 1 && result.data[result.DataLength - 1] == 0)
                {
                    result.DataLength--;
                }
            }

            // overflow check
            int lastPos = MaxLength - 1;

            // overflow if initial value was +ve but ++ caused a sign
            // change to negative.
            if ((bi1.data[lastPos] & 0x80000000) == 0 &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw new ArithmeticException("Overflow in ++.");
            }

            return result;
        }

        // ***********************************************************************
        // Overloading of subtraction operator
        // ***********************************************************************
        public static BigInteger operator -(BigInteger bi1, BigInteger bi2)
        {
            BigInteger result = new BigInteger();

            result.DataLength = (bi1.DataLength > bi2.DataLength) ? bi1.DataLength : bi2.DataLength;

            long carryIn = 0;
            for (int i = 0; i < result.DataLength; i++)
            {
                long diff;

                diff = bi1.data[i] - (long)bi2.data[i] - carryIn;
                result.data[i] = (uint)(diff & 0xFFFFFFFF);

                if (diff < 0)
                {
                    carryIn = 1;
                }
                else
                {
                    carryIn = 0;
                }
            }

            // roll over to negative
            if (carryIn != 0)
            {
                for (int i = result.DataLength; i < MaxLength; i++)
                {
                    result.data[i] = 0xFFFFFFFF;
                }

                result.DataLength = MaxLength;
            }

            // fixed in v1.03 to give correct datalength for a - (-b)
            while (result.DataLength > 1 && result.data[result.DataLength - 1] == 0)
            {
                result.DataLength--;
            }

            // overflow check
            int lastPos = MaxLength - 1;
            if ((bi1.data[lastPos] & 0x80000000) != (bi2.data[lastPos] & 0x80000000) &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw new ArithmeticException();
            }

            return result;
        }

        // ***********************************************************************
        // Overloading of the unary -- operator
        // ***********************************************************************
        public static BigInteger operator --(BigInteger bi1)
        {
            BigInteger result = new BigInteger(bi1);

            long val;
            bool carryIn = true;
            int index = 0;

            while (carryIn && index < MaxLength)
            {
                val = result.data[index];
                val--;

                result.data[index] = (uint)(val & 0xFFFFFFFF);

                if (val >= 0)
                {
                    carryIn = false;
                }

                index++;
            }

            if (index > result.DataLength)
            {
                result.DataLength = index;
            }

            while (result.DataLength > 1 && result.data[result.DataLength - 1] == 0)
            {
                result.DataLength--;
            }

            // overflow check
            int lastPos = MaxLength - 1;

            // overflow if initial value was -ve but -- caused a sign
            // change to positive.
            if ((bi1.data[lastPos] & 0x80000000) != 0 &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw new ArithmeticException("Underflow in --.");
            }

            return result;
        }

        // ***********************************************************************
        // Overloading of multiplication operator
        // ***********************************************************************
        public static BigInteger operator *(BigInteger bi1, BigInteger bi2)
        {
            int lastPos = MaxLength - 1;
            bool bi1Neg = false, bi2Neg = false;

            // take the absolute value of the inputs
            try
            {
                if ((bi1.data[lastPos] & 0x80000000) != 0) // bi1 negative
                {
                    bi1Neg = true;
                    bi1 = -bi1;
                }

                if ((bi2.data[lastPos] & 0x80000000) != 0) // bi2 negative
                {
                    bi2Neg = true;
                    bi2 = -bi2;
                }
            }
            catch (Exception)
            {
            }

            BigInteger result = new BigInteger();

            // multiply the absolute values
            try
            {
                for (int i = 0; i < bi1.DataLength; i++)
                {
                    if (bi1.data[i] == 0)
                    {
                        continue;
                    }

                    ulong mcarry = 0;
                    for (int j = 0, k = i; j < bi2.DataLength; j++, k++)
                    {
                        // k = i + j
                        ulong val = (bi1.data[i] * (ulong)bi2.data[j]) +
                                     result.data[k] + mcarry;

                        result.data[k] = (uint)(val & 0xFFFFFFFF);
                        mcarry = val >> 32;
                    }

                    if (mcarry != 0)
                    {
                        result.data[i + bi2.DataLength] = (uint)mcarry;
                    }
                }
            }
            catch (Exception)
            {
                throw new ArithmeticException("Multiplication overflow.");
            }

            result.DataLength = bi1.DataLength + bi2.DataLength;
            if (result.DataLength > MaxLength)
            {
                result.DataLength = MaxLength;
            }

            while (result.DataLength > 1 && result.data[result.DataLength - 1] == 0)
            {
                result.DataLength--;
            }

            // overflow check (result is -ve)
            if ((result.data[lastPos] & 0x80000000) != 0)
            {
                if (bi1Neg != bi2Neg && result.data[lastPos] == 0x80000000) // different sign
                {
                    // handle the special case where multiplication produces
                    // a max negative number in 2's complement.
                    if (result.DataLength == 1)
                    {
                        return result;
                    }

                    bool isMaxNeg = true;
                    for (int i = 0; i < result.DataLength - 1 && isMaxNeg; i++)
                    {
                        if (result.data[i] != 0)
                        {
                            isMaxNeg = false;
                        }
                    }

                    if (isMaxNeg)
                    {
                        return result;
                    }
                }

                throw new ArithmeticException("Multiplication overflow.");
            }

            // if input has different signs, then result is -ve
            if (bi1Neg != bi2Neg)
            {
                return -result;
            }

            return result;
        }

        // ***********************************************************************
        // Overloading of unary << operators
        // ***********************************************************************
        public static BigInteger operator <<(BigInteger bi1, int shiftVal)
        {
            BigInteger result = new BigInteger(bi1);
            result.DataLength = ShiftLeft(result.data, shiftVal);

            return result;
        }

        // least significant bits at lower part of buffer
        private static int ShiftLeft(uint[] buffer, int shiftVal)
        {
            int shiftAmount = 32;
            int bufLen = buffer.Length;

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
            {
                bufLen--;
            }

            for (int count = shiftVal; count > 0;)
            {
                if (count < shiftAmount)
                {
                    shiftAmount = count;
                }

                // Console.WriteLine("shiftAmount = {0}", shiftAmount);
                ulong carry = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    ulong val = ((ulong)buffer[i]) << shiftAmount;
                    val |= carry;

                    buffer[i] = (uint)(val & 0xFFFFFFFF);
                    carry = val >> 32;
                }

                if (carry != 0)
                {
                    if (bufLen + 1 <= buffer.Length)
                    {
                        buffer[bufLen] = (uint)carry;
                        bufLen++;
                    }
                }

                count -= shiftAmount;
            }

            return bufLen;
        }

        // ***********************************************************************
        // Overloading of unary >> operators
        // ***********************************************************************
        public static BigInteger operator >>(BigInteger bi1, int shiftVal)
        {
            BigInteger result = new BigInteger(bi1);
            result.DataLength = ShiftRight(result.data, shiftVal);

            if ((bi1.data[MaxLength - 1] & 0x80000000) != 0) // negative
            {
                for (int i = MaxLength - 1; i >= result.DataLength; i--)
                {
                    result.data[i] = 0xFFFFFFFF;
                }

                uint mask = 0x80000000;
                for (int i = 0; i < 32; i++)
                {
                    if ((result.data[result.DataLength - 1] & mask) != 0)
                    {
                        break;
                    }

                    result.data[result.DataLength - 1] |= mask;
                    mask >>= 1;
                }

                result.DataLength = MaxLength;
            }

            return result;
        }

        private static int ShiftRight(uint[] buffer, int shiftVal)
        {
            int shiftAmount = 32;
            int invShift = 0;
            int bufLen = buffer.Length;

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
            {
                bufLen--;
            }

            // Console.WriteLine("bufLen = " + bufLen + " buffer.Length = " + buffer.Length);
            for (int count = shiftVal; count > 0;)
            {
                if (count < shiftAmount)
                {
                    shiftAmount = count;
                    invShift = 32 - shiftAmount;
                }

                // Console.WriteLine("shiftAmount = {0}", shiftAmount);
                ulong carry = 0;
                for (int i = bufLen - 1; i >= 0; i--)
                {
                    ulong val = ((ulong)buffer[i]) >> shiftAmount;
                    val |= carry;

                    carry = ((ulong)buffer[i]) << invShift;
                    buffer[i] = (uint)val;
                }

                count -= shiftAmount;
            }

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
            {
                bufLen--;
            }

            return bufLen;
        }

        // ***********************************************************************
        // Overloading of the NOT operator (1's complement)
        // ***********************************************************************
        public static BigInteger operator ~(BigInteger bi1)
        {
            BigInteger result = new BigInteger(bi1);

            for (int i = 0; i < MaxLength; i++)
            {
                result.data[i] = ~bi1.data[i];
            }

            result.DataLength = MaxLength;

            while (result.DataLength > 1 && result.data[result.DataLength - 1] == 0)
            {
                result.DataLength--;
            }

            return result;
        }

        // ***********************************************************************
        // Overloading of the NEGATE operator (2's complement)
        // ***********************************************************************
        public static BigInteger operator -(BigInteger bi1)
        {
            // handle neg of zero separately since it'll cause an overflow
            // if we proceed.
            if (bi1.DataLength == 1 && bi1.data[0] == 0)
            {
                return new BigInteger();
            }

            BigInteger result = new BigInteger(bi1);

            // 1's complement
            for (int i = 0; i < MaxLength; i++)
            {
                result.data[i] = ~bi1.data[i];
            }

            // add one to result of 1's complement
            long val, carry = 1;
            int index = 0;

            while (carry != 0 && index < MaxLength)
            {
                val = result.data[index];
                val++;

                result.data[index] = (uint)(val & 0xFFFFFFFF);
                carry = val >> 32;

                index++;
            }

            if ((bi1.data[MaxLength - 1] & 0x80000000) == (result.data[MaxLength - 1] & 0x80000000))
            {
                throw new ArithmeticException("Overflow in negation.\n");
            }

            result.DataLength = MaxLength;

            while (result.DataLength > 1 && result.data[result.DataLength - 1] == 0)
            {
                result.DataLength--;
            }

            return result;
        }

        // ***********************************************************************
        // Overloading of equality operator
        // ***********************************************************************
        public static bool operator ==(BigInteger bi1, BigInteger bi2)
        {
            if (ReferenceEquals(null, bi1))
            {
                return ReferenceEquals(null, bi2);
            }

            return bi1.Equals(bi2);
        }

        public static bool operator !=(BigInteger bi1, BigInteger bi2)
        {
            return !bi1.Equals(bi2);
        }

        public override bool Equals(object o)
        {
            BigInteger bi = (BigInteger)o;

            if (bi == null || this.DataLength != bi.DataLength)
            {
                return false;
            }

            for (int i = 0; i < this.DataLength; i++)
            {
                if (this.data[i] != bi.data[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        // ***********************************************************************
        // Overloading of inequality operator
        // ***********************************************************************
        public static bool operator >(BigInteger bi1, BigInteger bi2)
        {
            int pos = MaxLength - 1;

            // bi1 is negative, bi2 is positive
            if ((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
            {
                return false;
            }

            // bi1 is positive, bi2 is negative
            if ((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
            {
                return true;
            }

            // same sign
            int len = (bi1.DataLength > bi2.DataLength) ? bi1.DataLength : bi2.DataLength;
            for (pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--)
            {
            }

            if (pos >= 0)
            {
                if (bi1.data[pos] > bi2.data[pos])
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        public static bool operator <(BigInteger bi1, BigInteger bi2)
        {
            int pos = MaxLength - 1;

            // bi1 is negative, bi2 is positive
            if ((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
            {
                return true;
            }

            // bi1 is positive, bi2 is negative
            if ((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
            {
                return false;
            }

            // same sign
            int len = (bi1.DataLength > bi2.DataLength) ? bi1.DataLength : bi2.DataLength;
            for (pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--)
            {
            }

            if (pos >= 0)
            {
                if (bi1.data[pos] < bi2.data[pos])
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        public static bool operator >=(BigInteger bi1, BigInteger bi2)
        {
            return bi1 == bi2 || bi1 > bi2;
        }

        public static bool operator <=(BigInteger bi1, BigInteger bi2)
        {
            return bi1 == bi2 || bi1 < bi2;
        }

        // ***********************************************************************
        // Private function that supports the division of two numbers with
        // a divisor that has more than 1 digit.
        //
        // Algorithm taken from [1]
        // ***********************************************************************
        private static void MultiByteDivide(BigInteger bi1, BigInteger bi2, BigInteger outQuotient, BigInteger outRemainder)
        {
            uint[] result = new uint[MaxLength];

            int remainderLen = bi1.DataLength + 1;
            uint[] remainder = new uint[remainderLen];

            uint mask = 0x80000000;
            uint val = bi2.data[bi2.DataLength - 1];
            int shift = 0, resultPos = 0;

            while (mask != 0 && (val & mask) == 0)
            {
                shift++;
                mask >>= 1;
            }

            // Console.WriteLine("shift = {0}", shift);
            // Console.WriteLine("Before bi1 Len = {0}, bi2 Len = {1}", bi1.dataLength, bi2.dataLength);
            for (int i = 0; i < bi1.DataLength; i++)
            {
                remainder[i] = bi1.data[i];
            }

            ShiftLeft(remainder, shift);
            bi2 = bi2 << shift;

            /*
            Console.WriteLine("bi1 Len = {0}, bi2 Len = {1}", bi1.dataLength, bi2.dataLength);
            Console.WriteLine("dividend = " + bi1 + "\ndivisor = " + bi2);
            for(int q = remainderLen - 1; q >= 0; q--)
                    Console.Write("{0:x2}", remainder[q]);
            Console.WriteLine();
            */

            int j = remainderLen - bi2.DataLength;
            int pos = remainderLen - 1;

            ulong firstDivisorByte = bi2.data[bi2.DataLength - 1];
            ulong secondDivisorByte = bi2.data[bi2.DataLength - 2];

            int divisorLen = bi2.DataLength + 1;
            uint[] dividendPart = new uint[divisorLen];

            while (j > 0)
            {
                ulong dividend = ((ulong)remainder[pos] << 32) + remainder[pos - 1];
                // Console.WriteLine("dividend = {0}", dividend);
                ulong qHat = dividend / firstDivisorByte;
                ulong rHat = dividend % firstDivisorByte;

                // Console.WriteLine("q_hat = {0:X}, r_hat = {1:X}", q_hat, r_hat);
                bool done = false;
                while (!done)
                {
                    done = true;

                    if (qHat == 0x100000000 ||
                       (qHat * secondDivisorByte) > ((rHat << 32) + remainder[pos - 2]))
                    {
                        qHat--;
                        rHat += firstDivisorByte;

                        if (rHat < 0x100000000)
                        {
                            done = false;
                        }
                    }
                }

                for (int h = 0; h < divisorLen; h++)
                {
                    dividendPart[h] = remainder[pos - h];
                }

                BigInteger kk = new BigInteger(dividendPart);
                BigInteger ss = bi2 * (long)qHat;

                // Console.WriteLine("ss before = " + ss);
                while (ss > kk)
                {
                    qHat--;
                    ss -= bi2;
                    // Console.WriteLine(ss);
                }

                BigInteger yy = kk - ss;

                // Console.WriteLine("ss = " + ss);
                // Console.WriteLine("kk = " + kk);
                // Console.WriteLine("yy = " + yy);
                for (int h = 0; h < divisorLen; h++)
                {
                    remainder[pos - h] = yy.data[bi2.DataLength - h];
                }

                /*
                Console.WriteLine("dividend = ");
                for(int q = remainderLen - 1; q >= 0; q--)
                        Console.Write("{0:x2}", remainder[q]);
                Console.WriteLine("\n************ q_hat = {0:X}\n", q_hat);
                */

                result[resultPos++] = (uint)qHat;

                pos--;
                j--;
            }

            outQuotient.DataLength = resultPos;
            int y = 0;
            for (int x = outQuotient.DataLength - 1; x >= 0; x--, y++)
            {
                outQuotient.data[y] = result[x];
            }

            for (; y < MaxLength; y++)
            {
                outQuotient.data[y] = 0;
            }

            while (outQuotient.DataLength > 1 && outQuotient.data[outQuotient.DataLength - 1] == 0)
            {
                outQuotient.DataLength--;
            }

            if (outQuotient.DataLength == 0)
            {
                outQuotient.DataLength = 1;
            }

            outRemainder.DataLength = ShiftRight(remainder, shift);

            for (y = 0; y < outRemainder.DataLength; y++)
            {
                outRemainder.data[y] = remainder[y];
            }

            for (; y < MaxLength; y++)
            {
                outRemainder.data[y] = 0;
            }
        }

        // ***********************************************************************
        // Private function that supports the division of two numbers with
        // a divisor that has only 1 digit.
        // ***********************************************************************
        private static void SingleByteDivide(BigInteger bi1, BigInteger bi2, BigInteger outQuotient, BigInteger outRemainder)
        {
            if (outQuotient == null)
            {
                throw new ArgumentNullException(nameof(outQuotient));
            }

            uint[] result = new uint[MaxLength];
            int resultPos = 0;

            // copy dividend to reminder
            for (int i = 0; i < MaxLength; i++)
            {
                outRemainder.data[i] = bi1.data[i];
            }

            outRemainder.DataLength = bi1.DataLength;

            while (outRemainder.DataLength > 1 && outRemainder.data[outRemainder.DataLength - 1] == 0)
            {
                outRemainder.DataLength--;
            }

            ulong divisor = bi2.data[0];
            int pos = outRemainder.DataLength - 1;
            ulong dividend = outRemainder.data[pos];

            // Console.WriteLine("divisor = " + divisor + " dividend = " + dividend);
            // Console.WriteLine("divisor = " + bi2 + "\ndividend = " + bi1);
            if (dividend >= divisor)
            {
                ulong quotient = dividend / divisor;
                result[resultPos++] = (uint)quotient;

                outRemainder.data[pos] = (uint)(dividend % divisor);
            }

            pos--;

            while (pos >= 0)
            {
                // Console.WriteLine(pos);
                dividend = ((ulong)outRemainder.data[pos + 1] << 32) + outRemainder.data[pos];
                ulong quotient = dividend / divisor;
                result[resultPos++] = (uint)quotient;

                outRemainder.data[pos + 1] = 0;
                outRemainder.data[pos--] = (uint)(dividend % divisor);
                // Console.WriteLine(">>>> " + bi1);
            }

            outQuotient.DataLength = resultPos;
            int j = 0;
            for (int i = outQuotient.DataLength - 1; i >= 0; i--, j++)
            {
                outQuotient.data[j] = result[i];
            }

            for (; j < MaxLength; j++)
            {
                outQuotient.data[j] = 0;
            }

            while (outQuotient.DataLength > 1 && outQuotient.data[outQuotient.DataLength - 1] == 0)
            {
                outQuotient.DataLength--;
            }

            if (outQuotient.DataLength == 0)
            {
                outQuotient.DataLength = 1;
            }

            while (outRemainder.DataLength > 1 && outRemainder.data[outRemainder.DataLength - 1] == 0)
            {
                outRemainder.DataLength--;
            }
        }

        // ***********************************************************************
        // Overloading of division operator
        // ***********************************************************************
        public static BigInteger operator /(BigInteger bi1, BigInteger bi2)
        {
            BigInteger quotient = new BigInteger();
            BigInteger remainder = new BigInteger();

            int lastPos = MaxLength - 1;
            bool divisorNeg = false, dividendNeg = false;

            if ((bi1.data[lastPos] & 0x80000000) != 0) // bi1 negative
            {
                bi1 = -bi1;
                dividendNeg = true;
            }

            if ((bi2.data[lastPos] & 0x80000000) != 0) // bi2 negative
            {
                bi2 = -bi2;
                divisorNeg = true;
            }

            if (bi1 < bi2)
            {
                return quotient;
            }

            if (bi2.DataLength == 1)
            {
                SingleByteDivide(bi1, bi2, quotient, remainder);
            }
            else
            {
                MultiByteDivide(bi1, bi2, quotient, remainder);
            }

            if (dividendNeg != divisorNeg)
            {
                return -quotient;
            }

            return quotient;
        }

        // ***********************************************************************
        // Overloading of modulus operator
        // ***********************************************************************
        public static BigInteger operator %(BigInteger bi1, BigInteger bi2)
        {
            BigInteger quotient = new BigInteger();
            BigInteger remainder = new BigInteger(bi1);

            int lastPos = MaxLength - 1;
            bool dividendNeg = false;

            if ((bi1.data[lastPos] & 0x80000000) != 0) // bi1 negative
            {
                bi1 = -bi1;
                dividendNeg = true;
            }

            if ((bi2.data[lastPos] & 0x80000000) != 0) // bi2 negative
            {
                bi2 = -bi2;
            }

            if (bi1 < bi2)
            {
                return remainder;
            }

            if (bi2.DataLength == 1)
            {
                SingleByteDivide(bi1, bi2, quotient, remainder);
            }
            else
            {
                MultiByteDivide(bi1, bi2, quotient, remainder);
            }

            if (dividendNeg)
            {
                return -remainder;
            }

            return remainder;
        }

        // ***********************************************************************
        // Overloading of bitwise AND operator
        // ***********************************************************************
        public static BigInteger operator &(BigInteger bi1, BigInteger bi2)
        {
            BigInteger result = new BigInteger();

            int len = (bi1.DataLength > bi2.DataLength) ? bi1.DataLength : bi2.DataLength;

            for (int i = 0; i < len; i++)
            {
                uint sum = bi1.data[i] & bi2.data[i];
                result.data[i] = sum;
            }

            result.DataLength = MaxLength;

            while (result.DataLength > 1 && result.data[result.DataLength - 1] == 0)
            {
                result.DataLength--;
            }

            return result;
        }

        // ***********************************************************************
        // Overloading of bitwise OR operator
        // ***********************************************************************
        public static BigInteger operator |(BigInteger bi1, BigInteger bi2)
        {
            BigInteger result = new BigInteger();

            int len = (bi1.DataLength > bi2.DataLength) ? bi1.DataLength : bi2.DataLength;

            for (int i = 0; i < len; i++)
            {
                uint sum = bi1.data[i] | bi2.data[i];
                result.data[i] = sum;
            }

            result.DataLength = MaxLength;

            while (result.DataLength > 1 && result.data[result.DataLength - 1] == 0)
            {
                result.DataLength--;
            }

            return result;
        }

        // ***********************************************************************
        // Overloading of bitwise XOR operator
        // ***********************************************************************
        public static BigInteger operator ^(BigInteger bi1, BigInteger bi2)
        {
            BigInteger result = new BigInteger();

            int len = (bi1.DataLength > bi2.DataLength) ? bi1.DataLength : bi2.DataLength;

            for (int i = 0; i < len; i++)
            {
                uint sum = bi1.data[i] ^ bi2.data[i];
                result.data[i] = sum;
            }

            result.DataLength = MaxLength;

            while (result.DataLength > 1 && result.data[result.DataLength - 1] == 0)
            {
                result.DataLength--;
            }

            return result;
        }

        // ***********************************************************************
        // Returns max(this, bi)
        // ***********************************************************************
        public BigInteger Max(BigInteger bi)
        {
            if (this > bi)
            {
                return new BigInteger(this);
            }

            return new BigInteger(bi);
        }

        // ***********************************************************************
        // Returns min(this, bi)
        // ***********************************************************************
        public BigInteger Min(BigInteger bi)
        {
            if (this < bi)
            {
                return new BigInteger(this);
            }

            return new BigInteger(bi);
        }

        // ***********************************************************************
        // Returns the absolute value
        // ***********************************************************************
        public BigInteger Abs()
        {
            if ((this.data[MaxLength - 1] & 0x80000000) != 0)
            {
                return -this;
            }

            return new BigInteger(this);
        }

        // ***********************************************************************
        // Returns a string representing the BigInteger in base 10.
        // ***********************************************************************
        public override string ToString()
        {
            return this.ToString(10);
        }

        // ***********************************************************************
        // Returns a string representing the BigInteger in sign-and-magnitude
        // format in the specified radix.
        //
        // Example
        // -------
        // If the value of BigInteger is -255 in base 10, then
        // ToString(16) returns "-FF"
        //
        // ***********************************************************************
        public string ToString(int radix)
        {
            if (radix < 2 || radix > 36)
            {
                throw new ArgumentException("Radix must be >= 2 and <= 36");
            }

            string charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string result = string.Empty;

            BigInteger a = this;

            bool negative = false;
            if ((a.data[MaxLength - 1] & 0x80000000) != 0)
            {
                negative = true;
                try
                {
                    a = -a;
                }
                catch (Exception)
                {
                }
            }

            BigInteger quotient = new BigInteger();
            BigInteger remainder = new BigInteger();
            BigInteger biRadix = new BigInteger(radix);

            if (a.DataLength == 1 && a.data[0] == 0)
            {
                result = "0";
            }
            else
            {
                while (a.DataLength > 1 || (a.DataLength == 1 && a.data[0] != 0))
                {
                    SingleByteDivide(a, biRadix, quotient, remainder);

                    if (remainder.data[0] < 10)
                    {
                        result = remainder.data[0] + result;
                    }
                    else
                    {
                        result = charSet[(int)remainder.data[0] - 10] + result;
                    }

                    a = quotient;
                }

                if (negative)
                {
                    result = "-" + result;
                }
            }

            return result;
        }

        // ***********************************************************************
        // Returns a hex string showing the contains of the BigInteger
        //
        // Examples
        // -------
        // 1) If the value of BigInteger is 255 in base 10, then
        //    ToHexString() returns "FF"
        //
        // 2) If the value of BigInteger is -255 in base 10, then
        //    ToHexString() returns ".....FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF01",
        //    which is the 2's complement representation of -255.
        //
        // ***********************************************************************
        public string ToHexString()
        {
            string result = this.data[this.DataLength - 1].ToString("X");

            for (int i = this.DataLength - 2; i >= 0; i--)
            {
                result += this.data[i].ToString("X8");
            }

            return result;
        }

        // ***********************************************************************
        // Modulo Exponentiation
        // ***********************************************************************
        public BigInteger ModPow(BigInteger exp, BigInteger n)
        {
            if ((exp.data[MaxLength - 1] & 0x80000000) != 0)
            {
                throw new ArithmeticException("Positive exponents only.");
            }

            BigInteger resultNum = 1;
            BigInteger tempNum;
            bool thisNegative = false;

            if ((this.data[MaxLength - 1] & 0x80000000) != 0) // negative this
            {
                tempNum = -this % n;
                thisNegative = true;
            }
            else
            {
                tempNum = this % n;  // ensures (tempNum * tempNum) < b^(2k)
            }

            if ((n.data[MaxLength - 1] & 0x80000000) != 0) // negative n
            {
                n = -n;
            }

            // calculate constant = b^(2k) / m
            BigInteger constant = new BigInteger();

            int i = n.DataLength << 1;
            constant.data[i] = 0x00000001;
            constant.DataLength = i + 1;

            constant = constant / n;
            int totalBits = exp.BitCount();
            int count = 0;

            // perform squaring and multiply exponentiation
            for (int pos = 0; pos < exp.DataLength; pos++)
            {
                uint mask = 0x01;
                // Console.WriteLine("pos = " + pos);
                for (int index = 0; index < 32; index++)
                {
                    if ((exp.data[pos] & mask) != 0)
                    {
                        resultNum = this.BarrettReduction(resultNum * tempNum, n, constant);
                    }

                    mask <<= 1;

                    tempNum = this.BarrettReduction(tempNum * tempNum, n, constant);

                    if (tempNum.DataLength == 1 && tempNum.data[0] == 1)
                    {
                        if (thisNegative && (exp.data[0] & 0x1) != 0) // odd exp
                        {
                            return -resultNum;
                        }

                        return resultNum;
                    }

                    count++;
                    if (count == totalBits)
                    {
                        break;
                    }
                }
            }

            if (thisNegative && (exp.data[0] & 0x1) != 0) // odd exp
            {
                return -resultNum;
            }

            return resultNum;
        }

        // ***********************************************************************
        // Fast calculation of modular reduction using Barrett's reduction.
        // Requires x < b^(2k), where b is the base.  In this case, base is
        // 2^32 (uint).
        //
        // Reference [4]
        // ***********************************************************************
        private BigInteger BarrettReduction(BigInteger x, BigInteger n, BigInteger constant)
        {
            int k = n.DataLength,
                kPlusOne = k + 1,
                kMinusOne = k - 1;

            BigInteger q1 = new BigInteger();

            // q1 = x / b^(k-1)
            for (int i = kMinusOne, j = 0; i < x.DataLength; i++, j++)
            {
                q1.data[j] = x.data[i];
            }

            q1.DataLength = x.DataLength - kMinusOne;
            if (q1.DataLength <= 0)
            {
                q1.DataLength = 1;
            }

            BigInteger q2 = q1 * constant;
            BigInteger q3 = new BigInteger();

            // q3 = q2 / b^(k+1)
            for (int i = kPlusOne, j = 0; i < q2.DataLength; i++, j++)
            {
                q3.data[j] = q2.data[i];
            }

            q3.DataLength = q2.DataLength - kPlusOne;
            if (q3.DataLength <= 0)
            {
                q3.DataLength = 1;
            }

            // r1 = x mod b^(k+1)
            // i.e. keep the lowest (k+1) words
            BigInteger r1 = new BigInteger();
            int lengthToCopy = (x.DataLength > kPlusOne) ? kPlusOne : x.DataLength;
            for (int i = 0; i < lengthToCopy; i++)
            {
                r1.data[i] = x.data[i];
            }

            r1.DataLength = lengthToCopy;

            // r2 = (q3 * n) mod b^(k+1)
            // partial multiplication of q3 and n
            BigInteger r2 = new BigInteger();
            for (int i = 0; i < q3.DataLength; i++)
            {
                if (q3.data[i] == 0)
                {
                    continue;
                }

                ulong mcarry = 0;
                int t = i;
                for (int j = 0; j < n.DataLength && t < kPlusOne; j++, t++)
                {
                    // t = i + j
                    ulong val = (q3.data[i] * (ulong)n.data[j]) +
                                 r2.data[t] + mcarry;

                    r2.data[t] = (uint)(val & 0xFFFFFFFF);
                    mcarry = val >> 32;
                }

                if (t < kPlusOne)
                {
                    r2.data[t] = (uint)mcarry;
                }
            }

            r2.DataLength = kPlusOne;
            while (r2.DataLength > 1 && r2.data[r2.DataLength - 1] == 0)
            {
                r2.DataLength--;
            }

            r1 -= r2;
            if ((r1.data[MaxLength - 1] & 0x80000000) != 0) // negative
            {
                BigInteger val = new BigInteger();
                val.data[kPlusOne] = 0x00000001;
                val.DataLength = kPlusOne + 1;
                r1 += val;
            }

            while (r1 >= n)
            {
                r1 -= n;
            }

            return r1;
        }

        // ***********************************************************************
        // Returns gcd(this, bi)
        // ***********************************************************************
        public BigInteger Gcd(BigInteger bi)
        {
            BigInteger x;
            BigInteger y;

            if ((this.data[MaxLength - 1] & 0x80000000) != 0) // negative
            {
                x = -this;
            }
            else
            {
                x = this;
            }

            if ((bi.data[MaxLength - 1] & 0x80000000) != 0) // negative
            {
                y = -bi;
            }
            else
            {
                y = bi;
            }

            BigInteger g = y;

            while (x.DataLength > 1 || (x.DataLength == 1 && x.data[0] != 0))
            {
                g = x;
                x = y % x;
                y = g;
            }

            return g;
        }

        // ***********************************************************************
        // Populates "this" with the specified amount of random bits
        // ***********************************************************************
        public void GenRandomBits(int bits, Random rand)
        {
            int dwords = bits >> 5;
            int remBits = bits & 0x1F;

            if (remBits != 0)
            {
                dwords++;
            }

            if (dwords > MaxLength)
            {
                throw new ArithmeticException("Number of required bits > maxLength.");
            }

            for (int i = 0; i < dwords; i++)
            {
                this.data[i] = (uint)(rand.NextDouble() * 0x100000000);
            }

            for (int i = dwords; i < MaxLength; i++)
            {
                this.data[i] = 0;
            }

            if (remBits != 0)
            {
                uint mask = (uint)(0x01 << (remBits - 1));
                this.data[dwords - 1] |= mask;

                mask = 0xFFFFFFFF >> (32 - remBits);
                this.data[dwords - 1] &= mask;
            }
            else
            {
                this.data[dwords - 1] |= 0x80000000;
            }

            this.DataLength = dwords;

            if (this.DataLength == 0)
            {
                this.DataLength = 1;
            }
        }

        // ***********************************************************************
        // Returns the position of the most significant bit in the BigInteger.
        //
        // Eg.  The result is 0, if the value of BigInteger is 0...0000 0000
        //      The result is 1, if the value of BigInteger is 0...0000 0001
        //      The result is 2, if the value of BigInteger is 0...0000 0010
        //      The result is 2, if the value of BigInteger is 0...0000 0011
        //
        // ***********************************************************************
        public int BitCount()
        {
            while (this.DataLength > 1 && this.data[this.DataLength - 1] == 0)
            {
                this.DataLength--;
            }

            uint value = this.data[this.DataLength - 1];
            uint mask = 0x80000000;
            int bits = 32;

            while (bits > 0 && (value & mask) == 0)
            {
                bits--;
                mask >>= 1;
            }

            bits += (this.DataLength - 1) << 5;

            return bits;
        }

        // ***********************************************************************
        // Returns the lowest 4 bytes of the BigInteger as an int.
        // ***********************************************************************
        public int IntValue()
        {
            return (int)this.data[0];
        }

        // ***********************************************************************
        // Returns the lowest 8 bytes of the BigInteger as a long.
        // ***********************************************************************
        public long LongValue()
        {
            long val = 0;

            val = this.data[0];
            try
            { // exception if maxLength = 1
                val |= (long)this.data[1] << 32;
            }
            catch (Exception)
            {
                if ((this.data[0] & 0x80000000) != 0) // negative
                {
                    val = (int)this.data[0];
                }
            }

            return val;
        }

        // ***********************************************************************
        // Computes the Jacobi Symbol for a and b.
        // Algorithm adapted from [3] and [4] with some optimizations
        // ***********************************************************************
        public static int Jacobi(BigInteger a, BigInteger b)
        {
            // Jacobi defined only for odd integers
            if ((b.data[0] & 0x1) == 0)
            {
                throw new ArgumentException("Jacobi defined only for odd integers.");
            }

            if (a >= b)
            {
                a %= b;
            }

            if (a.DataLength == 1 && a.data[0] == 0)
            {
                return 0;  // a == 0
            }

            if (a.DataLength == 1 && a.data[0] == 1)
            {
                return 1;  // a == 1
            }

            if (a < 0)
            {
                if (((b - 1).data[0] & 0x2) == 0) // if( (((b-1) >> 1).data[0] & 0x1) == 0)
                {
                    return Jacobi(-a, b);
                }

                return -Jacobi(-a, b);
            }

            int e = 0;
            for (int index = 0; index < a.DataLength; index++)
            {
                uint mask = 0x01;

                for (int i = 0; i < 32; i++)
                {
                    if ((a.data[index] & mask) != 0)
                    {
                        index = a.DataLength;      // to break the outer loop
                        break;
                    }

                    mask <<= 1;
                    e++;
                }
            }

            BigInteger a1 = a >> e;

            int s = 1;
            if ((e & 0x1) != 0 && ((b.data[0] & 0x7) == 3 || (b.data[0] & 0x7) == 5))
            {
                s = -1;
            }

            if ((b.data[0] & 0x3) == 3 && (a1.data[0] & 0x3) == 3)
            {
                s = -s;
            }

            if (a1.DataLength == 1 && a1.data[0] == 1)
            {
                return s;
            }

            return s * Jacobi(b % a1, a1);
        }

        // ***********************************************************************
        // Returns the modulo inverse of this.  Throws ArithmeticException if
        // the inverse does not exist.  (i.e. gcd(this, modulus) != 1)
        // ***********************************************************************
        public BigInteger ModInverse(BigInteger modulus)
        {
            BigInteger[] p = { 0, 1 };
            BigInteger[] q = new BigInteger[2];    // quotients
            BigInteger[] r = { 0, 0 };             // remainders

            int step = 0;

            BigInteger a = modulus;
            BigInteger b = this;

            while (b.DataLength > 1 || (b.DataLength == 1 && b.data[0] != 0))
            {
                BigInteger quotient = new BigInteger();
                BigInteger remainder = new BigInteger();

                if (step > 1)
                {
                    BigInteger pval = (p[0] - (p[1] * q[0])) % modulus;
                    p[0] = p[1];
                    p[1] = pval;
                }

                if (b.DataLength == 1)
                {
                    SingleByteDivide(a, b, quotient, remainder);
                }
                else
                {
                    MultiByteDivide(a, b, quotient, remainder);
                }

                /*
                Console.WriteLine(quotient.dataLength);
                Console.WriteLine("{0} = {1}({2}) + {3}  p = {4}", a.ToString(10),
                                  b.ToString(10), quotient.ToString(10), remainder.ToString(10),
                                  p[1].ToString(10));
                */

                q[0] = q[1];
                r[0] = r[1];
                q[1] = quotient;
                r[1] = remainder;

                a = b;
                b = remainder;

                step++;
            }

            if (r[0].DataLength > 1 || (r[0].DataLength == 1 && r[0].data[0] != 1))
            {
                throw new ArithmeticException("No inverse!");
            }

            BigInteger result = (p[0] - (p[1] * q[0])) % modulus;

            if ((result.data[MaxLength - 1] & 0x80000000) != 0)
            {
                result += modulus;  // get the least positive modulus
            }

            return result;
        }

        // ***********************************************************************
        // Returns the value of the BigInteger as a byte array.  The lowest
        // index contains the MSB.
        // ***********************************************************************
        public byte[] GetBytes()
        {
            int numBits = this.BitCount();

            int numBytes = numBits >> 3;
            if ((numBits & 0x7) != 0)
            {
                numBytes++;
            }

            byte[] result = new byte[numBytes];

            // Console.WriteLine(result.Length);
            int pos = 0;
            uint tempVal, val = this.data[this.DataLength - 1];

            if ((tempVal = val >> 24 & 0xFF) != 0)
            {
                result[pos++] = (byte)tempVal;
            }

            if ((tempVal = val >> 16 & 0xFF) != 0)
            {
                result[pos++] = (byte)tempVal;
            }

            if ((tempVal = val >> 8 & 0xFF) != 0)
            {
                result[pos++] = (byte)tempVal;
            }

            if ((tempVal = val & 0xFF) != 0)
            {
                result[pos++] = (byte)tempVal;
            }

            for (int i = this.DataLength - 2; i >= 0; i--, pos += 4)
            {
                val = this.data[i];
                result[pos + 3] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos + 2] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos + 1] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos] = (byte)(val & 0xFF);
            }

            return result;
        }

        // ***********************************************************************
        // Sets the value of the specified bit to 1
        // The Least Significant Bit position is 0.
        // ***********************************************************************
        public void SetBit(uint bitNum)
        {
            uint bytePos = bitNum >> 5;             // divide by 32
            byte bitPos = (byte)(bitNum & 0x1F);    // get the lowest 5 bits

            uint mask = (uint)1 << bitPos;
            this.data[bytePos] |= mask;

            if (bytePos >= this.DataLength)
            {
                this.DataLength = (int)bytePos + 1;
            }
        }

        // ***********************************************************************
        // Sets the value of the specified bit to 0
        // The Least Significant Bit position is 0.
        // ***********************************************************************
        public void UnsetBit(uint bitNum)
        {
            uint bytePos = bitNum >> 5;

            if (bytePos < this.DataLength)
            {
                byte bitPos = (byte)(bitNum & 0x1F);

                uint mask = (uint)1 << bitPos;
                uint mask2 = 0xFFFFFFFF ^ mask;

                this.data[bytePos] &= mask2;

                if (this.DataLength > 1 && this.data[this.DataLength - 1] == 0)
                {
                    this.DataLength--;
                }
            }
        }

        // ***********************************************************************
        // Returns a value that is equivalent to the integer square root
        // of the BigInteger.
        //
        // The integer square root of "this" is defined as the largest integer n
        // such that (n * n) <= this
        //
        // ***********************************************************************
        public BigInteger Sqrt()
        {
            uint numBits = (uint)this.BitCount();

            if ((numBits & 0x1) != 0) // odd number of bits
            {
                numBits = (numBits >> 1) + 1;
            }
            else
            {
                numBits = numBits >> 1;
            }

            uint bytePos = numBits >> 5;
            byte bitPos = (byte)(numBits & 0x1F);

            uint mask;

            BigInteger result = new BigInteger();
            if (bitPos == 0)
            {
                mask = 0x80000000;
            }
            else
            {
                mask = (uint)1 << bitPos;
                bytePos++;
            }

            result.DataLength = (int)bytePos;

            for (int i = (int)bytePos - 1; i >= 0; i--)
            {
                while (mask != 0)
                {
                    // guess
                    result.data[i] ^= mask;

                    // undo the guess if its square is larger than this
                    if ((result * result) > this)
                    {
                        result.data[i] ^= mask;
                    }

                    mask >>= 1;
                }

                mask = 0x80000000;
            }

            return result;
        }

        // ***********************************************************************
        // Returns the k_th number in the Lucas Sequence reduced modulo n.
        //
        // Uses index doubling to speed up the process.  For example, to calculate V(k),
        // we maintain two numbers in the sequence V(n) and V(n+1).
        //
        // To obtain V(2n), we use the identity
        //      V(2n) = (V(n) * V(n)) - (2 * Q^n)
        // To obtain V(2n+1), we first write it as
        //      V(2n+1) = V((n+1) + n)
        // and use the identity
        //      V(m+n) = V(m) * V(n) - Q * V(m-n)
        // Hence,
        //      V((n+1) + n) = V(n+1) * V(n) - Q^n * V((n+1) - n)
        //                   = V(n+1) * V(n) - Q^n * V(1)
        //                   = V(n+1) * V(n) - Q^n * P
        //
        // We use k in its binary expansion and perform index doubling for each
        // bit position.  For each bit position that is set, we perform an
        // index doubling followed by an index addition.  This means that for V(n),
        // we need to update it to V(2n+1).  For V(n+1), we need to update it to
        // V((2n+1)+1) = V(2*(n+1))
        //
        // This function returns
        // [0] = U(k)
        // [1] = V(k)
        // [2] = Q^n
        //
        // Where U(0) = 0 % n, U(1) = 1 % n
        //       V(0) = 2 % n, V(1) = P % n
        // ***********************************************************************
        public static BigInteger[] LucasSequence(BigInteger p, BigInteger q, BigInteger k, BigInteger n)
        {
            if (k.DataLength == 1 && k.data[0] == 0)
            {
                BigInteger[] result = new BigInteger[3];

                result[0] = 0;
                result[1] = 2 % n;
                result[2] = 1 % n;
                return result;
            }

            // calculate constant = b^(2k) / m
            // for Barrett Reduction
            BigInteger constant = new BigInteger();

            int nLen = n.DataLength << 1;
            constant.data[nLen] = 0x00000001;
            constant.DataLength = nLen + 1;

            constant = constant / n;

            // calculate values of s and t
            int s = 0;

            for (int index = 0; index < k.DataLength; index++)
            {
                uint mask = 0x01;

                for (int i = 0; i < 32; i++)
                {
                    if ((k.data[index] & mask) != 0)
                    {
                        index = k.DataLength;      // to break the outer loop
                        break;
                    }

                    mask <<= 1;
                    s++;
                }
            }

            BigInteger t = k >> s;

            // Console.WriteLine("s = " + s + " t = " + t);
            return LucasSequenceHelper(p, q, t, n, constant, s);
        }

        // ***********************************************************************
        // Performs the calculation of the kth term in the Lucas Sequence.
        // For details of the algorithm, see reference [9].
        //
        // k must be odd.  i.e LSB == 1
        // ***********************************************************************
        private static BigInteger[] LucasSequenceHelper(BigInteger p, BigInteger q, BigInteger k, BigInteger n, BigInteger constant, int s)
        {
            BigInteger[] result = new BigInteger[3];

            if ((k.data[0] & 0x00000001) == 0)
            {
                throw new ArgumentException("Argument k must be odd.");
            }

            int numbits = k.BitCount();
            uint mask = (uint)0x1 << ((numbits & 0x1F) - 1);

            // v = v0, v1 = v1, u1 = u1, Q_k = Q^0
            BigInteger v = 2 % n, qK = 1 % n,
                       v1 = p % n, u1 = qK;
            bool flag = true;

            for (int i = k.DataLength - 1; i >= 0; i--) // iterate on the binary expansion of k
            {
                // Console.WriteLine("round");
                while (mask != 0)
                {
                    if (i == 0 && mask == 0x00000001) // last bit
                    {
                        break;
                    }

                    if ((k.data[i] & mask) != 0) // bit is set
                    {
                        // index doubling with addition
                        u1 = (u1 * v1) % n;

                        v = ((v * v1) - (p * qK)) % n;
                        v1 = n.BarrettReduction(v1 * v1, n, constant);
                        v1 = (v1 - ((qK * q) << 1)) % n;

                        if (flag)
                        {
                            flag = false;
                        }
                        else
                        {
                            qK = n.BarrettReduction(qK * qK, n, constant);
                        }

                        qK = (qK * q) % n;
                    }
                    else
                    {
                        // index doubling
                        u1 = ((u1 * v) - qK) % n;

                        v1 = ((v * v1) - (p * qK)) % n;
                        v = n.BarrettReduction(v * v, n, constant);
                        v = (v - (qK << 1)) % n;

                        if (flag)
                        {
                            qK = q % n;
                            flag = false;
                        }
                        else
                        {
                            qK = n.BarrettReduction(qK * qK, n, constant);
                        }
                    }

                    mask >>= 1;
                }

                mask = 0x80000000;
            }

            // at this point u1 = u(n+1) and v = v(n)
            // since the last bit always 1, we need to transform u1 to u(2n+1) and v to v(2n+1)
            u1 = ((u1 * v) - qK) % n;
            v = ((v * v1) - (p * qK)) % n;
            if (flag)
            {
                flag = false;
            }
            else
            {
                qK = n.BarrettReduction(qK * qK, n, constant);
            }

            qK = (qK * q) % n;

            for (int i = 0; i < s; i++)
            {
                // index doubling
                u1 = (u1 * v) % n;
                v = ((v * v) - (qK << 1)) % n;

                if (flag)
                {
                    qK = q % n;
                    flag = false;
                }
                else
                {
                    qK = n.BarrettReduction(qK * qK, n, constant);
                }
            }

            result[0] = u1;
            result[1] = v;
            result[2] = qK;

            return result;
        }
    }
}
