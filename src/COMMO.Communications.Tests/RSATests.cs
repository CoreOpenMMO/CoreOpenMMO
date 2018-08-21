namespace COMMO.Communications.Tests {
	using System;
	using System.Linq;
	using System.Text;
	using COMMO.Communications.Criptography;
	using NUnit.Framework;

	[TestFixture]
	public sealed class RSATests {

		[Test]
		public void Encrypt_ThrowsIfMessageTooLong() {
			var data = new byte[RSA.LengthOfEncodedData + 1];

			Assert.That(
				() => RSA.EncryptWithOTCKeys(data),
				Throws.ArgumentException);
		}

		[Test]
		public void EncryptDecrypt() {
			var message = "lol";
			var encoded = Encoding.UTF8.GetBytes(message);

			var encrypted = RSA.EncryptWithOTCKeys(encoded);
			var decrypted = RSA.DecryptWithOTCKeys(encrypted);

			var decoded = Encoding.UTF8.GetString(decrypted);

			Assert.AreEqual(
				expected: message,
				actual: decoded);
		}


		[Test]
		public void TryEncryptTryDecrypt() {
			var message = "lol";
			var encoded = Encoding.UTF8.GetBytes(message);

			var encryptionBuffer = new byte[RSA.LengthOfEncodedData];
			var encrypted = RSA.TryEncryptWithOTCKeys(
				data: encoded,
				destination: encryptionBuffer,
				bytesWritten: out var _);

			var decryptionBuffer = new byte[RSA.LengthOfEncodedData];
			var decrypted = RSA.TryDecryptWithOTCKeys(
				data: encryptionBuffer,
				destination: decryptionBuffer,
				bytesWritten: out var bytesWritten);

			var relevantData = new ReadOnlySpan<byte>(
				array: decryptionBuffer,
				start: 0,
				length: bytesWritten);
			var decoded = Encoding.UTF8.GetString(relevantData);

			Assert.IsTrue(encrypted);
			Assert.IsTrue(decrypted);
			Assert.AreEqual(
				expected: message,
				actual: decoded);
		}

		[Test]
		public void LegacyConsistency_LegacyEncrypt_LegacyDecrypt() {
			var message = "lol";
			var encoded = Encoding.UTF8.GetBytes(message);

			var encrypted = LegacyEncrypt(encoded);
			var decrypted = LegacyDecrypt(encrypted, encoded.Length);

			var decoded = Encoding.UTF8.GetString(decrypted);

			Assert.AreEqual(
				actual: decoded,
				expected: message);
		}

		[Test]
		public void LegacyConsistency_NewEncrypt_LegacyDecrypt() {
			var message = "lol";
			var encoded = Encoding.UTF8.GetBytes(message);

			var padSize = RSA.MaximumDataLengthBeforeEncryption - encoded.Length;
			var padded = new byte[RSA.MaximumDataLengthBeforeEncryption];
			padded = encoded.ToArray();
			Array.Resize(ref padded, RSA.MaximumDataLengthBeforeEncryption);
			//Array.Copy(
			//	sourceArray: encoded,
			//	sourceIndex: 0,
			//	destinationArray: padded,
			//	destinationIndex: padSize,
			//	length: encoded.Length);

			var encrypted = RSA.EncryptWithOTCKeys(padded);
			var decrypted = LegacyDecrypt(encrypted, encoded.Length);

			var decoded = Encoding.UTF8.GetString(decrypted);

			Assert.AreEqual(
				actual: decoded,
				expected: message);
		}

		[Test]
		public void LegacyConsistency_LegacyEncrypt_NewDecrypt() {
			var message = "lol";
			var encoded = Encoding.UTF8.GetBytes(message);

			var encrypted = LegacyEncrypt(encoded);
			var decrypted = RSA.DecryptWithOTCKeys(encrypted);

			var decoded = Encoding.UTF8.GetString(decrypted);

			Assert.AreEqual(
				actual: decoded,
				expected: message);
		}

		/// <summary>
		/// Coz legacy code encrypts in-place.
		/// </summary>
		private static byte[] LegacyEncrypt(Span<byte> data) {
			var messageCopy = data.ToArray();
			Array.Resize(ref messageCopy, RSA.LengthOfEncodedData);

			var encrypted = COMMO.Security.Encryption.Rsa.Encrypt(
				buffer: ref messageCopy,
				position: 0,
				useCipValues: false);

			if (!encrypted)
				throw new InvalidOperationException();

			return messageCopy;
		}

		private static byte[] LegacyDecrypt(Span<byte> data, int length) {
			var dataCopy = data.ToArray();

			var decrypted = COMMO.Security.Encryption.Rsa.Decrypt(
				buffer: ref dataCopy,
				position: data.Length,
				length: data.Length,
				useCipValues: false);

			if (!decrypted)
				throw new InvalidOperationException();

			var relevantData = new Span<byte>(dataCopy, 0, length).ToArray();
			return relevantData;
		}
	}
}
