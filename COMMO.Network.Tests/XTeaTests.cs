using Xunit;

namespace COMMO.Network.Tests {

	public sealed class XTeaTests {
		private readonly uint[] _key = new uint[] { 3442030272, 2364789040, 1503299581, 3670909886 };
		private readonly byte[] _message = new byte[] { 123, 0, 20, 20, 0, 48, 10, 66, 101, 109, 32, 118, 105, 110, 100, 111 };
		private readonly byte[] _encryptedMessage = new byte[] { 163, 35, 242, 102, 150, 173, 252, 174, 83, 54, 209, 248, 35, 145, 205, 229 };

    [Fact]
		public void XTeaEncrypt() {
			var encryptedMessage = XTea
				.Encrypt(_message, _key)
				.ToArray();

			Assert.Equal(
				expected: _encryptedMessage,
				actual: encryptedMessage);
		}

		[Fact]
		public void XTeaDecrypt() {
			var decryptedMsg = XTea
				.Decrypt(_encryptedMessage, _key)
				.ToArray();

			Assert.Equal(
				expected: _message,
				actual: decryptedMsg);
		}
	}
}
