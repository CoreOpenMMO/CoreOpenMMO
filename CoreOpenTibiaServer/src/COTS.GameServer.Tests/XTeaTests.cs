using COTS.Infra.CrossCutting.Network.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace COTS.GameServer.Tests {
    [TestClass]
    public sealed class XTeaTests {

		#region PrivateStructures
		private struct _FakeCryptMessage {
			public byte[] Buffer;
			public int HeaderPosition;
			public int Length;
			public const int Rounds = 32;
			public const uint Delta = 0x9e3779b9;

			public _FakeCryptMessage(byte[] msg, int headpos) {
				Buffer = msg;
				HeaderPosition = headpos;
				Length = msg.Length;
			}

			public uint GetUInt32FromIndex(int startIndex) {
				return BitConverter.ToUInt32(Buffer, startIndex);
			}

			public void OverwriteBytes(int pos, byte[] newBytes) {
				for (var i = 0; i < newBytes.Length; i++) {
					Buffer[pos + i] = newBytes[i];
				}
			}
		}

		private byte[] _PreMadeMessage(int size, bool random = false) {
			var newArray = new byte[size];

			if (random) {
				var randomI = new Random();
				for (var k = 0; k < size; k++) {
					newArray[k] = BitConverter.GetBytes(randomI.Next(minValue: 0, maxValue: 255))[0];
				}
			}
			else {
				#region ManualData
				newArray[0] = 123;
				newArray[1] = 0;
				newArray[2] = 20;
				newArray[3] = 20;
				newArray[4] = 0;
				newArray[5] = 48;
				newArray[6] = 10;
				newArray[7] = 66;
				newArray[8] = 101;
				newArray[9] = 109;
				newArray[10] = 32;
				newArray[11] = 118;
				newArray[12] = 105;
				newArray[13] = 110;
				newArray[14] = 100;
				newArray[15] = 111;
				newArray[16] = 32;
				newArray[17] = 97;
				newArray[18] = 111;
				newArray[19] = 32;
				newArray[20] = 67;
				newArray[21] = 79;
				newArray[22] = 84;
				newArray[23] = 83;
				newArray[24] = 33;
				newArray[25] = 40;
				newArray[26] = 23;
				newArray[27] = 0;
				newArray[28] = 49;
				newArray[29] = 50;
				newArray[30] = 51;
				newArray[31] = 10;
				newArray[32] = 10;
				newArray[33] = 10;
				newArray[34] = 50;
				newArray[35] = 49;
				newArray[36] = 50;
				newArray[37] = 49;
				newArray[38] = 55;
				newArray[39] = 56;
				newArray[40] = 49;
				newArray[41] = 52;
				newArray[42] = 50;
				newArray[43] = 49;
				newArray[44] = 52;
				newArray[45] = 55;
				newArray[46] = 49;
				newArray[47] = 48;
				newArray[48] = 55;
				newArray[49] = 53;
				newArray[50] = 51;
				newArray[51] = 100;
				newArray[52] = 1;
				newArray[53] = 0;
				newArray[54] = 4;
				newArray[55] = 0;
				newArray[56] = 67;
				newArray[57] = 79;
				newArray[58] = 84;
				newArray[59] = 83;
				newArray[60] = 9;
				newArray[61] = 0;
				newArray[62] = 49;
				newArray[63] = 50;
				newArray[64] = 55;
				newArray[65] = 46;
				newArray[66] = 48;
				newArray[67] = 46;
				newArray[68] = 48;
				newArray[69] = 46;
				newArray[70] = 49;
				newArray[71] = 4;
				newArray[72] = 28;
				newArray[73] = 0;
				newArray[74] = 4;
				newArray[75] = 0;
				newArray[76] = 8;
				newArray[77] = 0;
				newArray[78] = 80;
				newArray[79] = 108;
				newArray[80] = 97;
				newArray[81] = 121;
				newArray[82] = 101;
				newArray[83] = 114;
				newArray[84] = 32;
				newArray[85] = 49;
				newArray[86] = 0;
				newArray[87] = 8;
				newArray[88] = 0;
				newArray[89] = 80;
				newArray[90] = 108;
				newArray[91] = 97;
				newArray[92] = 121;
				newArray[93] = 101;
				newArray[94] = 114;
				newArray[95] = 32;
				newArray[96] = 50;
				newArray[97] = 0;
				newArray[98] = 8;
				newArray[99] = 0;
				newArray[100] = 80;
				newArray[101] = 108;
				newArray[102] = 97;
				newArray[103] = 121;
				newArray[104] = 101;
				newArray[105] = 114;
				newArray[106] = 32;
				newArray[107] = 51;
				newArray[108] = 0;
				newArray[109] = 8;
				newArray[110] = 0;
				newArray[111] = 80;
				newArray[112] = 108;
				newArray[113] = 97;
				newArray[114] = 121;
				newArray[115] = 101;
				newArray[116] = 114;
				newArray[117] = 32;
				newArray[118] = 52;
				newArray[119] = 0;
				newArray[120] = 1;
				newArray[121] = 0;
				newArray[122] = 0;
				newArray[123] = 0;
				newArray[124] = 0;
				newArray[125] = 51;
				newArray[126] = 51;
				newArray[127] = 51;
				#endregion
			}
			return newArray;
		}

		private byte[] _slowFunctionalEncrypt(ref _FakeCryptMessage msg, uint[] key) {
			uint x_sum = 0;
			var indexCurr = 0;
			var indexCurrPlusOne = 0;
			uint current = 0;
			uint currentPlusOne = 0;

			for (var pos = 0; pos < msg.Length / 4; pos += 2, x_sum = 0) {
				//Run rounds of XTea
				for (var count = _FakeCryptMessage.Rounds; count > 0; count--) {
					indexCurr = msg.HeaderPosition + (pos * sizeof(uint));
					indexCurrPlusOne = msg.HeaderPosition + ((pos + 1) * sizeof(uint));

					current = msg.GetUInt32FromIndex(indexCurr);
					currentPlusOne = msg.GetUInt32FromIndex(indexCurrPlusOne);

					msg.OverwriteBytes(indexCurr,
						BitConverter.GetBytes(
							current +
							(
								(currentPlusOne << 4 ^ currentPlusOne >> 5) + currentPlusOne ^ x_sum + key[x_sum & 3]
							)
						)
					);

					x_sum += _FakeCryptMessage.Delta;

					current = msg.GetUInt32FromIndex(indexCurr);
					currentPlusOne = msg.GetUInt32FromIndex(indexCurrPlusOne);

					msg.OverwriteBytes(indexCurrPlusOne,
						BitConverter.GetBytes(
							currentPlusOne +
							(
								(current << 4 ^ current >> 5) + current ^ x_sum + key[x_sum >> 11 & 3]
							)
						)
					);
				}
			}
			return msg.Buffer;
		}
		#endregion

		[TestMethod]
        public void XTeaEncryption_Equals_Slow_Functional_Version() {
			var key = new uint[4];

			key[0] = 3442030272;
			key[1] = 2364789040;
			key[2] = 1503299581;
			key[3] = 3670909886;

			var fakeMsg = _PreMadeMessage(128);

			var slowMsg = new _FakeCryptMessage(fakeMsg, 0);
			var slowCryptedMsg = _slowFunctionalEncrypt(ref slowMsg, key);

			var fastCryptedMsg = XTea.XteaEncrypt(fakeMsg, key);
			CollectionAssert.AreEqual(slowCryptedMsg, fastCryptedMsg);
		}
	}
}