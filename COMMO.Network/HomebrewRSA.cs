using System;
using System.Numerics;

namespace COMMO.Network {

	public class HomebrewRSA {
		private static BigInteger _n;
		private static BigInteger _d;
		private static BigInteger _me = new BigInteger(65537);

		public static bool SetKey(string p, string q) {
			Console.WriteLine("Setting up RSA encyption");

			BigInteger mP, mQ;
			try {
				mP = BigInteger.Parse(p);
				mQ = BigInteger.Parse(q);
			} catch (Exception) {
				Console.WriteLine("P,Q value could not be parsed!");
				return false;
			}

			_n = mP * mQ;

			BigInteger mod = (mP - 1) * (mQ - 1);

			_d = _me.ModInverse(mod);
			return true;
		}

		public static void Encrypt(ref byte[] buffer, int index) {
			byte[] temp = new byte[128];
			Array.Copy(buffer, index, temp, 0, 128);

			var input = new BigInteger(temp);
			var output = BigInteger.ModPow(input, _me, _n);

			Array.Copy(GetPaddedValue(output), 0, buffer, index, 128);
		}

		public static void Decrypt(ref byte[] buffer, int index) {
			byte[] temp = new byte[128];
			Array.Copy(buffer, index, temp, 0, 128);

			var input = new BigInteger(temp);
			var output = BigInteger.ModPow(input, _d, _n);

			Array.Copy(GetPaddedValue(output), 0, buffer, index, 128);
		}

		private static byte[] GetPaddedValue(BigInteger value) {
			byte[] result = value.ToByteArray();

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