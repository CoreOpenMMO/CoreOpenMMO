using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using SharpServer.Server.Utils;

namespace SharpServer.Server.Packets
{
    public static class Rsa
    {
        #region Constants
        static Big otServerP = new Big("14299623962416399520070177382898895550795403345466153217470516082934737582776038882967213386204600674145392845853859217990626450972452084065728686565928113", 10);
        static Big otServerQ = new Big("7630979195970404721891201847792002125535401292779123937207447574596692788513647179235335529307251350570728407373705564708871762033017096809910315212884101", 10);

        static Big otServerD = new Big("46730330223584118622160180015036832148732986808519344675210555262940258739805766860224610646919605860206328024326703361630109888417839241959507572247284807035235569619173792292786907845791904955103601652822519121908367187885509270025388641700821735345222087940578381210879116823013776808975766851829020659073", 10);
        static Big otServerM = new Big("109120132967399429278860960508995541528237502902798129123468757937266291492576446330739696001110603907230888610072655818825358503429057592827629436413108566029093628212635953836686562675849720620786279431090218017681061521755056710823876476444260558147179707119674283982419152118103759076030616683978566631413", 10);
        static Big otServerE = new Big("65537", 10);
        static Big otServerDP = new Big("11141736698610418925078406669215087697114858422461871124661098818361832856659225315773346115219673296375487744032858798960485665997181641221483584094519937", 10);
        static Big otServerDQ = new Big("4886309137722172729208909250386672706991365415741885286554321031904881408516947737562153523770981322408725111241551398797744838697461929408240938369297973", 10);
        static Big otServerInverseQ = new Big("5610960212328996596431206032772162188356793727360507633581722789998709372832546447914318965787194031968482458122348411654607397146261039733584248408719418", 10);
        
        #endregion

        #region Public Functions

        public static bool Encrypt(ref byte[] buffer, int position)
        {
            return true;
            //return Encrypt(otServerE, otServerM, ref buffer, position);
        }

        //public static bool Encrypt(BigInteger e, BigInteger m, ref byte[] buffer, int position)
        //{
        //    byte[] temp = new byte[128];

        //    Array.Copy(buffer, position, temp, 0, 128);

        //    BigInteger input = new BigInteger(temp);
        //    BigInteger output = input.modPow(e, m);

        //    Array.Copy(GetPaddedValue(output), 0, buffer, position, 128);

        //    return true;
        //}

        public static bool Decrypt(ref List<byte> buffer, int position, int length)
        {
            var bufferArray = buffer.ToArray();

            if (length - position < 128)
                return false;

            byte[] temp = new byte[128];
            Array.Copy(bufferArray, position, temp, 0, 128);

            Big input = new Big(temp);
            Big output;

            Big m1 = input.modPow(otServerDP, otServerP);
            Big m2 = input.modPow(otServerDQ, otServerQ);
            Big h;

            if (m2 > m1)
            {
                h = otServerP - ((m2 - m1) * otServerInverseQ % otServerP);
                output = m2 + otServerQ * h;
            }
            else
            {

                h = (m1 - m2) * otServerInverseQ % otServerP;

                output = m2 + otServerQ * h;
            }

            Array.Copy(GetPaddedValue(output), 0, bufferArray, position, 128);
            buffer = bufferArray.ToList();
            return true;
        }

        public static bool Decrypt(ref List<byte> buffer, int position)
        {
            try
            {
                var bufferArray = buffer.ToArray();
                var stream = new BigInteger(bufferArray);
                BigInteger n, d;
                BigInteger.TryParse("109120132967399429278860960508995541528237502902798129123468757937266291492576446330739696001110603907230888610072655818825358503429057592827629436413108566029093628212635953836686562675849720620786279431090218017681061521755056710823876476444260558147179707119674283982419152118103759076030616683978566631413", out n);
                BigInteger.TryParse("46730330223584118622160180015036832148732986808519344675210555262940258739805766860224610646919605860206328024326703361630109888417839241959507572247284807035235569619173792292786907845791904955103601652822519121908367187885509270025388641700821735345222087940578381210879116823013776808975766851829020659073", out d);
                
                Array.Copy(BigInteger.ModPow(stream, n, d).ToByteArray(), 0, bufferArray, position, 128);
                buffer = bufferArray.ToList();
                return true;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return false;
            }

        }


        public static bool Decryp3(ref List<byte> buffer, int position)
        {
            try
            {
                RSAOpenSsl rsa = new RSAOpenSsl();

                buffer = rsa.Decrypt(buffer.ToArray(), null).ToList();

                return true;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return false;
            }

        }

        public static bool Decrypt2(ref List<byte> buffer, int position, int length)
        {
            var bufferArray = buffer.ToArray();
            byte[] temp = new byte[128];

            Array.Copy(bufferArray, position, temp, 0, 128);
            
            Big input = new Big(temp);

            Big res = input.modPow(otServerD, otServerM);
            
            Array.Copy(GetPaddedValue(res), 0, bufferArray, position, 128);
            buffer = bufferArray.ToList();
            return true;

        }

        #endregion

        #region Private Functions

        private static byte[] GetPaddedValue(Big value)
        {
            byte[] result = value.getBytes();

            int length = (1024 >> 3);
            if (result.Length >= length)
                return result;

            // left-pad 0x00 value on the result (same integer, correct length)
            byte[] padded = new byte[length];
            System.Buffer.BlockCopy(result, 0, padded, (length - result.Length), result.Length);
            // temporary result may contain decrypted (plaintext) data, clear it
            Array.Clear(result, 0, result.Length);
            return padded;
        }

        #endregion
    }
}
