// <copyright file="Rsa.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Security.Encryption
{
    using System;

    public static class Rsa
    {
        static readonly BigInteger OtServerP = new BigInteger("14299623962416399520070177382898895550795403345466153217470516082934737582776038882967213386204600674145392845853859217990626450972452084065728686565928113", 10);
        static readonly BigInteger OtServerQ = new BigInteger("7630979195970404721891201847792002125535401292779123937207447574596692788513647179235335529307251350570728407373705564708871762033017096809910315212884101", 10);
        static BigInteger otServerD = new BigInteger("46730330223584118622160180015036832148732986808519344675210555262940258739805766860224610646919605860206328024326703361630109888417839241959507572247284807035235569619173792292786907845791904955103601652822519121908367187885509270025388641700821735345222087940578381210879116823013776808975766851829020659073", 10);
        static readonly BigInteger OtServerM = new BigInteger("109120132967399429278860960508995541528237502902798129123468757937266291492576446330739696001110603907230888610072655818825358503429057592827629436413108566029093628212635953836686562675849720620786279431090218017681061521755056710823876476444260558147179707119674283982419152118103759076030616683978566631413", 10);
        static readonly BigInteger OtServerE = new BigInteger("65537", 10);
        static readonly BigInteger OtServerDp = new BigInteger("11141736698610418925078406669215087697114858422461871124661098818361832856659225315773346115219673296375487744032858798960485665997181641221483584094519937", 10);
        static readonly BigInteger OtServerDq = new BigInteger("4886309137722172729208909250386672706991365415741885286554321031904881408516947737562153523770981322408725111241551398797744838697461929408240938369297973", 10);
        static readonly BigInteger OtServerInverseQ = new BigInteger("5610960212328996596431206032772162188356793727360507633581722789998709372832546447914318965787194031968482458122348411654607397146261039733584248408719418", 10);

        static BigInteger cipM = new BigInteger("124710459426827943004376449897985582167801707960697037164044904862948569380850421396904597686953877022394604239428185498284169068581802277612081027966724336319448537811441719076484340922854929273517308661370727105382899118999403808045846444647284499123164879035103627004668521005328367415259939915284902061793", 10);

        static readonly BigInteger CipP = new BigInteger("12017580013707233233987537782574702577133548287527131234152948150506251412291888866940292054989907714155267326586216043845592229084368540020196135619327879", 10);
        static readonly BigInteger CipQ = new BigInteger("11898921368616868351880508246112101394478760265769325412746398405473436969889506919017477758618276066588858607419440134394668095105156501566867770737187273", 10);
        static readonly BigInteger CipE = new BigInteger("65537", 10);
        static BigInteger cipN;
        static BigInteger cipD;
        static BigInteger cipP1;
        static BigInteger cipQ1;
        static BigInteger cipPq1;
        static BigInteger cipDp;
        static BigInteger cipDq;
        static BigInteger cipInverseQ;

        static readonly object CipNLock = new object();
        static readonly object CipDLock = new object();
        static readonly object CipP1Lock = new object();
        static readonly object CipQ1Lock = new object();
        static readonly object CipPq1Lock = new object();
        static readonly object CipDpLock = new object();
        static readonly object CipDqLock = new object();
        static readonly object CipInverseQLock = new object();

        static BigInteger CipN
        {
            get
            {
                if (cipN == null)
                {
                    lock (CipNLock)
                    {
                        if (cipN == null)
                        {
                            // n = p * q
                            cipN = CipP * CipQ;
                        }
                    }
                }

                return cipN;
            }
        }

        static BigInteger CipP1
        {
            get
            {
                if (cipP1 == null)
                {
                    lock (CipP1Lock)
                    {
                        if (cipP1 == null)
                        {
                            cipP1 = CipP - 1;
                        }
                    }
                }

                return cipP1;
            }
        }

        static BigInteger CipQ1
        {
            get
            {
                if (cipQ1 == null)
                {
                    lock (CipQ1Lock)
                    {
                        if (cipQ1 == null)
                        {
                            cipQ1 = CipQ - 1;
                        }
                    }
                }

                return cipQ1;
            }
        }

        static BigInteger CipPq1
        {
            get
            {
                if (cipPq1 == null)
                {
                    lock (CipPq1Lock)
                    {
                        if (cipPq1 == null)
                        {
                            cipPq1 = CipP1 * CipQ1;
                        }
                    }
                }

                return cipPq1;
            }
        }

        static BigInteger CipD
        {
            get
            {
                if (cipD == null)
                {
                    lock (CipDLock)
                    {
                        if (cipD == null)
                        {
                            // m_d = m_e^-1 mod (p - 1)(q - 1)
                            cipD = CipE.ModInverse(CipPq1);
                        }
                    }
                }

                return cipD;
            }
        }

        static BigInteger CipDp
        {
            get
            {
                if (cipDp == null)
                {
                    lock (CipDpLock)
                    {
                        if (cipDp == null)
                        {
                            cipDp = CipD % CipP1;
                        }
                    }
                }

                return cipDp;
            }
        }

        static BigInteger CipDq
        {
            get
            {
                if (cipDq == null)
                {
                    lock (CipDqLock)
                    {
                        if (cipDq == null)
                        {
                            cipDq = CipD % CipQ1;
                        }
                    }
                }

                return cipDq;
            }
        }

        static BigInteger CipInverseQ
        {
            get
            {
                if (cipInverseQ == null)
                {
                    lock (CipInverseQLock)
                    {
                        if (cipInverseQ == null)
                        {
                            cipInverseQ = CipQ.ModInverse(CipP);
                        }
                    }
                }

                return cipInverseQ;
            }
        }

        public static bool Encrypt(ref byte[] buffer, int position, bool useCipValues = true)
        {
            return useCipValues ? Encrypt(CipE, CipN, ref buffer, position) : Encrypt(OtServerE, OtServerM, ref buffer, position);
        }

        public static bool Encrypt(BigInteger e, BigInteger m, ref byte[] buffer, int position)
        {
            byte[] temp = new byte[128];

            Array.Copy(buffer, position, temp, 0, 128);

            BigInteger input = new BigInteger(temp);
            BigInteger output = input.ModPow(e, m);

            Array.Copy(GetPaddedValue(output), 0, buffer, position, 128);

            return true;
        }

        public static bool Decrypt(ref byte[] buffer, int position, int length, bool useCipValues = true)
        {
            // if (length - position != 128)
            //    return false;
            position = length - 128;

            byte[] temp = new byte[128];
            Array.Copy(buffer, position, temp, 0, 128);

            BigInteger input = new BigInteger(temp);
            BigInteger output;

            BigInteger m1 = useCipValues ? input.ModPow(CipDp, CipP) : input.ModPow(OtServerDp, OtServerP);
            BigInteger m2 = useCipValues ? input.ModPow(CipDq, CipQ) : input.ModPow(OtServerDq, OtServerQ);
            BigInteger h;

            if (useCipValues)
            {
                if (m2 > m1)
                {
                    h = CipP - (((m2 - m1) * CipInverseQ) % CipP);
                }
                else
                {
                    h = ((m1 - m2) * CipInverseQ) % CipP;
                }

                output = m2 + (CipQ * h);
            }
            else
            {
                if (m2 > m1)
                {
                    h = OtServerP - (((m2 - m1) * OtServerInverseQ) % OtServerP);
                }
                else
                {
                    h = ((m1 - m2) * OtServerInverseQ) % OtServerP;
                }

                output = m2 + (OtServerQ * h);
            }

            Array.Copy(GetPaddedValue(output), 0, buffer, position, 128);
            return true;
        }

        private static byte[] GetPaddedValue(BigInteger value)
        {
            byte[] result = value.GetBytes();

            int length = 1024 >> 3;
            if (result.Length >= length)
            {
                return result;
            }

            // left-pad 0x00 value on the result (same integer, correct length)
            byte[] padded = new byte[length];
            Buffer.BlockCopy(result, 0, padded, length - result.Length, result.Length);
            // temporary result may contain decrypted (plaintext) data, clear it
            Array.Clear(result, 0, result.Length);
            return padded;
        }
    }
}
