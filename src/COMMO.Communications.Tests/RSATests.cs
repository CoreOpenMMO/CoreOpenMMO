namespace COMMO.Communications.Tests {
	using System;
	using System.Text;
	using COMMO.Communications.Criptography;
	using NUnit.Framework;

	[TestFixture]
	public sealed class RSATests {

		[Test]
		public void Encrypt_ThrowsIfMessageTooLong() {
			var data = new byte[RSA.MessageLength + 1];

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

			var encryptionBuffer = new byte[RSA.MessageLength];
			var encrypted = RSA.TryEncryptWithOTCKeys(
				data: encoded,
				destination: encryptionBuffer,
				bytesWritten: out var _);

			var decryptionBuffer = new byte[RSA.MessageLength];
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
		public void LegacyCompoatibility_Encrypt() {
			var message = "lol";
			var encoded = Encoding.UTF8.GetBytes(message);

			var legacyEncrypted = LegacyEncrypt(encoded);
			var newEncrypted = RSA.EncryptWithOTCKeys(encoded);

			Assert.That(
				newEncrypted,
				Is.EquivalentTo(legacyEncrypted));
		}

		/// <summary>
		/// Coz legacy code encrypts in-place.
		/// </summary>
		private static byte[] LegacyEncrypt(Span<byte> message) {
			var messageCopy = message.ToArray();

			throw new NotImplementedException();

			return messageCopy;
		}
	}
}
