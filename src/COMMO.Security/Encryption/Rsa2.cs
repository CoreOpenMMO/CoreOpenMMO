using System;

namespace COMMO.Security.Encryption {
	public static class Rsa2
    {
        private static BigInteger _n;
        private static BigInteger _d;
        private static readonly BigInteger _me = new BigInteger("65537", 10);

		private static readonly string _p = "14299623962416399520070177382898895550795403345466153217470516082934737582776038882967213386204600674145392845853859217990626450972452084065728686565928113";
		private static readonly string _q = "7630979195970404721891201847792002125535401292779123937207447574596692788513647179235335529307251350570728407373705564708871762033017096809910315212884101";

		public static bool SetKey() {
			Console.WriteLine("Setting up RSA encyption");

			BigInteger mP, mQ;
			try {
				mP = new BigInteger(_p, 10);
				mQ = new BigInteger(_q, 10);
			}
			catch (Exception) {
				Console.WriteLine("P,Q value could not be parsed!");
				return false;
			}

			_n = mP * mQ;

			var mod = (mP - 1) * (mQ - 1);

			_d = _me.ModInverse(mod);
			return true;
		}

		public static bool Encrypt(ref byte[] buffer, int index)
        {
			SetKey();

            byte[] temp = new byte[128];
            Array.Copy(buffer, index, temp, 0, 128);

            var input = new BigInteger(temp);
            var output = input.ModPow(_me, _n);

            Array.Copy(GetPaddedValue(output), 0, buffer, index, 128);
			return true;
        }

        public static bool Decrypt(ref byte[] buffer, int index, int lenght)
        {
			SetKey();

            byte[] temp = new byte[128];
            Array.Copy(buffer, index, temp, 0, 128);
            
            var input = new BigInteger(temp);
            var output = input.ModPow(_d, _n);

            Array.Copy(GetPaddedValue(output), 0, buffer, index, 128);
			return true;
        }

        private static byte[] GetPaddedValue(BigInteger value)
        {
            byte[] result = value.GetBytes();

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