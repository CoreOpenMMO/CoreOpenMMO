using System;

namespace COTS.Infra.CrossCutting.Security
{
    public class Rsa
    {
        protected static BigInteger N;
        protected static BigInteger D;
        protected static BigInteger Me = new BigInteger("65537", 10);

        public static bool SetKey(string p, string q)
        {
            Console.WriteLine("Setting up RSA encyption");

            BigInteger mP, mQ;
            try
            {
                mP = new BigInteger(p, 10);
                mQ = new BigInteger(q, 10);
            }
            catch (Exception)
            {
                Console.WriteLine("P,Q value could not be parsed!");
                return false;
            }

            N = mP * mQ;

            BigInteger mod = (mP - 1) * (mQ - 1);

            D = Me.ModInverse(mod);
            return true;
        }

        public static void Encrypt(ref byte[] buffer, int index)
        {
            byte[] temp = new byte[128];
            Array.Copy(buffer, index, temp, 0, 128);

            BigInteger input = new BigInteger(temp);
            BigInteger output = input.modPow(Me, N);

            Array.Copy(GetPaddedValue(output), 0, buffer, index, 128);
        }

        public static void Decrypt(ref byte[] buffer, int index)
        {
            byte[] temp = new byte[128];
            Array.Copy(buffer, index, temp, 0, 128);

            BigInteger input = new BigInteger(temp);
            BigInteger output = input.modPow(D, N);

            Array.Copy(GetPaddedValue(output), 0, buffer, index, 128);
        }

        private static byte[] GetPaddedValue(BigInteger value)
        {
            byte[] result = value.getBytes();

            const int length = (1024 >> 3);
            if (result.Length >= length)
                return result;

            // left-pad 0x00 value on the result (same integer, correct length)
            byte[] padded = new byte[length];
            Buffer.BlockCopy(result, 0, padded, (length - result.Length), result.Length);
            // temporary result may contain decrypted (plaintext) data, clear it
            Array.Clear(result, 0, result.Length);
            return padded;
        }

    }
}