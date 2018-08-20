namespace COMMO.Communications.Tests {
	using System.Text;
	using COMMO.Communications.Criptography;
	using NUnit.Framework;

	[TestFixture]
	public sealed class RSATests {

		[Test]
		public void EncryptDecryptTest() {
			var message = "lol";
			var encoded = Encoding.UTF8.GetBytes(message);

			var encrypted = RSA.Encrypt(encoded);
			var decrypted = RSA.Decrypt(encrypted);

			var decoded = Encoding.UTF8.GetString(decrypted);

			Assert.AreEqual(
				expected: message,
				actual: decoded);
		}
	}
}
