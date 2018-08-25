namespace COMMO.Communications.Tests {
	using System;
	using System.Linq;
	using COMMO.Communications.Criptography;
	using NUnit.Framework;
	using OldRsa = COMMO.Security.Encryption.Rsa;
	using OldRsa2 = COMMO.Security.Encryption.Rsa2;

	[TestFixture]
	public sealed class OTCRSATests {

		[Test]
		public void EncryptDecrypt(
			[Values(
			new byte[] { },
			new byte[] { 0 },
			new byte[] { 1 },
			new byte[] { 0, 1 },
			new byte[] { 1, 0 },
			new byte[] { 1, 1 })] byte[] data) {

			var padded = data.ToArray();
			Array.Resize(ref padded, 128);
			var encrypted = OTCRSA.Encrypt(padded);

			var decrypted = OTCRSA.Decrypt(encrypted)
				.ToArray()
				.Take(data.Length)
				.ToArray();

			Assert.That(decrypted.ToArray(), Is.EquivalentTo(data.ToArray()));
		}

		[Test]
		public void PadThenEncrypt_DecryptThenUnpad(
			[Values(
			new byte[] { },
			new byte[] { 0 },
			new byte[] { 1 },
			new byte[] { 0, 1 },
			new byte[] { 1, 0 },
			new byte[] { 1, 1 })] byte[] data) {

			var paddingSize = OTCRSA.PadThenEncrypt(
				data: data,
				out var encryptedData);

			var decryptedData = OTCRSA.DecryptThenUnpad(
				data: encryptedData,
				paddingSize: paddingSize);

			Assert.That(decryptedData.ToArray(), Is.EquivalentTo(data.ToArray()));
		}

		[Test]
		public void WriteMe() {
			var ughz = new byte[0];
			OldRsa.Encrypt(ref ughz, 0);
			OldRsa2.Decrypt(ref ughz, 0, 0);
		}
	}
}
