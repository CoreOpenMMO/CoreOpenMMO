// <copyright file="Rsa.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace COMMO.Security.Encryption {
	public static class Rsa {
		private static readonly BigInteger _otServerP = new BigInteger("14299623962416399520070177382898895550795403345466153217470516082934737582776038882967213386204600674145392845853859217990626450972452084065728686565928113", 10);
		private static readonly BigInteger _otServerQ = new BigInteger("7630979195970404721891201847792002125535401292779123937207447574596692788513647179235335529307251350570728407373705564708871762033017096809910315212884101", 10);
		private static readonly BigInteger _otServerD = new BigInteger("46730330223584118622160180015036832148732986808519344675210555262940258739805766860224610646919605860206328024326703361630109888417839241959507572247284807035235569619173792292786907845791904955103601652822519121908367187885509270025388641700821735345222087940578381210879116823013776808975766851829020659073", 10);
		private static readonly BigInteger _otServerM = new BigInteger("109120132967399429278860960508995541528237502902798129123468757937266291492576446330739696001110603907230888610072655818825358503429057592827629436413108566029093628212635953836686562675849720620786279431090218017681061521755056710823876476444260558147179707119674283982419152118103759076030616683978566631413", 10);
		private static readonly BigInteger _otServerE = new BigInteger("65537", 10);
		private static readonly BigInteger _otServerDp = new BigInteger("11141736698610418925078406669215087697114858422461871124661098818361832856659225315773346115219673296375487744032858798960485665997181641221483584094519937", 10);
		private static readonly BigInteger _otServerDq = new BigInteger("4886309137722172729208909250386672706991365415741885286554321031904881408516947737562153523770981322408725111241551398797744838697461929408240938369297973", 10);
		private static readonly BigInteger _otServerInverseQ = new BigInteger("5610960212328996596431206032772162188356793727360507633581722789998709372832546447914318965787194031968482458122348411654607397146261039733584248408719418", 10);
		private static readonly BigInteger _cipM = new BigInteger("124710459426827943004376449897985582167801707960697037164044904862948569380850421396904597686953877022394604239428185498284169068581802277612081027966724336319448537811441719076484340922854929273517308661370727105382899118999403808045846444647284499123164879035103627004668521005328367415259939915284902061793", 10);
		private static readonly BigInteger _cipP = new BigInteger("12017580013707233233987537782574702577133548287527131234152948150506251412291888866940292054989907714155267326586216043845592229084368540020196135619327879", 10);
		private static readonly BigInteger _cipQ = new BigInteger("11898921368616868351880508246112101394478760265769325412746398405473436969889506919017477758618276066588858607419440134394668095105156501566867770737187273", 10);
		private static readonly BigInteger _cipE = new BigInteger("65537", 10);
		private static BigInteger _cipN;
		private static BigInteger _cipD;
		private static BigInteger _cipP1;
		private static BigInteger _cipQ1;
		private static BigInteger _cipPq1;
		private static BigInteger _cipDp;
		private static BigInteger _cipDq;
		private static BigInteger _cipInverseQ;
		private static readonly object _cipNLock = new object();
		private static readonly object _cipDLock = new object();
		private static readonly object _cipP1Lock = new object();
		private static readonly object _cipQ1Lock = new object();
		private static readonly object _cipPq1Lock = new object();
		private static readonly object _cipDpLock = new object();
		private static readonly object _cipDqLock = new object();
		private static readonly object _cipInverseQLock = new object();

		private static BigInteger CipN {
			get {
				if (_cipN == null) {
					lock (_cipNLock) {
						if (_cipN == null) {
							// n = p * q
							_cipN = _cipP * _cipQ;
						}
					}
				}

				return _cipN;
			}
		}

		private static BigInteger CipP1 {
			get {
				if (_cipP1 == null) {
					lock (_cipP1Lock) {
						if (_cipP1 == null) {
							_cipP1 = _cipP - 1;
						}
					}
				}

				return _cipP1;
			}
		}

		private static BigInteger CipQ1 {
			get {
				if (_cipQ1 == null) {
					lock (_cipQ1Lock) {
						if (_cipQ1 == null) {
							_cipQ1 = _cipQ - 1;
						}
					}
				}

				return _cipQ1;
			}
		}

		private static BigInteger CipPq1 {
			get {
				if (_cipPq1 == null) {
					lock (_cipPq1Lock) {
						if (_cipPq1 == null) {
							_cipPq1 = CipP1 * CipQ1;
						}
					}
				}

				return _cipPq1;
			}
		}

		private static BigInteger CipD {
			get {
				if (_cipD == null) {
					lock (_cipDLock) {
						if (_cipD == null) {
							// m_d = m_e^-1 mod (p - 1)(q - 1)
							_cipD = _cipE.ModInverse(CipPq1);
						}
					}
				}

				return _cipD;
			}
		}

		private static BigInteger CipDp {
			get {
				if (_cipDp == null) {
					lock (_cipDpLock) {
						if (_cipDp == null) {
							_cipDp = CipD % CipP1;
						}
					}
				}

				return _cipDp;
			}
		}

		private static BigInteger CipDq {
			get {
				if (_cipDq == null) {
					lock (_cipDqLock) {
						if (_cipDq == null) {
							_cipDq = CipD % CipQ1;
						}
					}
				}

				return _cipDq;
			}
		}

		private static BigInteger CipInverseQ {
			get {
				if (_cipInverseQ == null) {
					lock (_cipInverseQLock) {
						if (_cipInverseQ == null) {
							_cipInverseQ = _cipQ.ModInverse(_cipP);
						}
					}
				}

				return _cipInverseQ;
			}
		}

		public static bool Encrypt(ref byte[] buffer, int position, bool useCipValues = true) {
			return useCipValues ? Encrypt(_cipE, CipN, ref buffer, position) : Encrypt(_otServerE, _otServerM, ref buffer, position);
		}

		public static bool Encrypt(BigInteger e, BigInteger m, ref byte[] buffer, int position) {
			byte[] temp = new byte[128];

			Array.Copy(buffer, position, temp, 0, 128);

			var input = new BigInteger(temp);
			var output = input.ModPow(e, m);

			Array.Copy(GetPaddedValue(output), 0, buffer, position, 128);

			return true;
		}

		public static bool Decrypt(ref byte[] buffer, int position, int length, bool useCipValues = true) {
			// if (length - position != 128)
			//    return false;
			position = length - 128;

			byte[] temp = new byte[128];
			Array.Copy(buffer, position, temp, 0, 128);

			var input = new BigInteger(temp);
			BigInteger output;

			var m1 = useCipValues ? input.ModPow(CipDp, _cipP) : input.ModPow(_otServerDp, _otServerP);
			var m2 = useCipValues ? input.ModPow(CipDq, _cipQ) : input.ModPow(_otServerDq, _otServerQ);
			BigInteger h;

			if (useCipValues) {
				if (m2 > m1) {
					h = _cipP - (((m2 - m1) * CipInverseQ) % _cipP);
				}
				else {
					h = ((m1 - m2) * CipInverseQ) % _cipP;
				}

				output = m2 + (_cipQ * h);
			}
			else {
				if (m2 > m1) {
					h = _otServerP - (((m2 - m1) * _otServerInverseQ) % _otServerP);
				}
				else {
					h = ((m1 - m2) * _otServerInverseQ) % _otServerP;
				}

				output = m2 + (_otServerQ * h);
			}

			Array.Copy(GetPaddedValue(output), 0, buffer, position, 128);
			return true;
		}

		private static byte[] GetPaddedValue(BigInteger value) {
			byte[] result = value.GetBytes();

			int length = 1024 >> 3;
			if (result.Length >= length) {
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
