using System.Numerics;

namespace COMMO.Network {

	public static class BigIntegerExtensions {
		public static BigInteger ModInverse(this BigInteger a, BigInteger n) {
			BigInteger i = n, v = 0, d = 1;
			while (a > 0) {
				BigInteger t = i / a, x = a;
				a = i % x;
				i = x;
				x = d;
				d = v - t * x;
				v = x;
			}
			v %= n;
			if (v < 0)
				v = (v + n) % n;
			return v;
		}
	}
}
