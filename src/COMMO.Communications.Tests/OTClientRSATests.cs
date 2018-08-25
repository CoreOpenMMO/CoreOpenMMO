namespace COMMO.Communications.Tests {
	using System;
	using System.Linq;
	using COMMO.Communications.Criptography;
	using NUnit.Framework;
	using OldRsa = COMMO.Security.Encryption.Rsa;
	using OldRsa2 = COMMO.Security.Encryption.Rsa2;

	[TestFixture]
	public sealed class OTClientRSATests {

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
			var encrypted = OTClientRSA.Encrypt(padded);

			var decrypted = OTClientRSA.Decrypt(encrypted)
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

			var paddingSize = OTClientRSA.PadThenEncrypt(
				data: data,
				out var encryptedData);

			var decryptedData = OTClientRSA.DecryptThenUnpad(
				data: encryptedData,
				paddingSize: paddingSize);

			Assert.That(decryptedData.ToArray(), Is.EquivalentTo(data.ToArray()));
		}


		// Legacy compatibility tests
		[Test]
		public void OldEncrypt_NewDecrypt(
			[Values(
			new byte[] { },
			new byte[] { 0 },
			new byte[] { 1 },
			new byte[] { 0, 1 },
			new byte[] { 1, 0 },
			new byte[] { 1, 1 })] byte[] data) {

			var paddedData = data.ToArray();
			Array.Resize(ref paddedData, OTClientRSA.DataLength);

			var paddingSize = OTClientRSA.DataLength - data.Length;

			OldRsa.Encrypt(
				buffer: ref paddedData,
				position: 0,
				useCipValues: false);

			var decryptedData = OTClientRSA.DecryptThenUnpad(
				data: paddedData,
				paddingSize: paddingSize);

			Assert.That(decryptedData.ToArray(), Is.EquivalentTo(data.ToArray()));
		}

		[Test]
		public void NewEncrypt_OldDecrypt(
			[Values(
			new byte[] { },
			new byte[] { 0 },
			new byte[] { 1 },
			new byte[] { 0, 1 },
			new byte[] { 1, 0 },
			new byte[] { 1, 1 })] byte[] data) {

			var paddingSize = OTClientRSA.PadThenEncrypt(
				data: data,
				output: out var encryptedData);

			var buffer = encryptedData.ToArray();

			OldRsa.Decrypt(
				buffer: ref buffer,
				position: 0,
				length: OTClientRSA.DataLength,
				useCipValues: false);

			Array.Resize(ref buffer, data.Length);

			Assert.That(buffer.ToArray(), Is.EquivalentTo(data.ToArray()));
		}

		[Test]
		public void OldEncrypt2_NewDecrypt(
			[Values(
			new byte[] { },
			new byte[] { 0 },
			new byte[] { 1 },
			new byte[] { 0, 1 },
			new byte[] { 1, 0 },
			new byte[] { 1, 1 })] byte[] data) {

			var paddedData = data.ToArray();
			Array.Resize(ref paddedData, OTClientRSA.DataLength);

			var paddingSize = OTClientRSA.DataLength - data.Length;

			OldRsa2.Encrypt(
				buffer: ref paddedData,
				index: 0);

			var decryptedData = OTClientRSA.DecryptThenUnpad(
				data: paddedData,
				paddingSize: paddingSize);

			Assert.That(decryptedData.ToArray(), Is.EquivalentTo(data.ToArray()));
		}

		[Test]
		public void NewEncrypt_OldDecrypt2(
			[Values(
			new byte[] { },
			new byte[] { 0 },
			new byte[] { 1 },
			new byte[] { 0, 1 },
			new byte[] { 1, 0 },
			new byte[] { 1, 1 })] byte[] data) {

			var paddingSize = OTClientRSA.PadThenEncrypt(
				data: data,
				output: out var encryptedData);

			var buffer = encryptedData.ToArray();

			OldRsa2.Decrypt(
				buffer: ref buffer,
				index: 0,
				lenght: data.Length);

			Array.Resize(ref buffer, data.Length);

			Assert.That(buffer.ToArray(), Is.EquivalentTo(data.ToArray()));
		}
	}
}
