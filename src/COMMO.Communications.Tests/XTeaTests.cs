namespace COMMO.Communications.Tests {
	using System;
	using System.Runtime.InteropServices;
	using COMMO.Communications.Criptography;
	using NUnit.Framework;

	[TestFixture]
	public sealed class XTeaTests {
		public const int MinimumMessageSize = 8;

		[Theory]
		public void Encrypt_IncorrectKeySize(
			[Range(from: 0, to: 32)] int keyLength
			) {

			Assume.That(keyLength != XTea.KeySizeInBytes);

			var message = new byte[MinimumMessageSize];
			var key = new byte[keyLength];

			Assert.Throws<ArgumentException>(
				() => XTea.Encrypt(
					message: message,
					key: key)
				);
		}


		[Test, Combinatorial]
		public void LegacyCompatibility_Encrypt(
			[Values(
			new UInt64[] { 0, 1},
			new UInt64[] { 0, 1, 2})] UInt64[] message,
			[Values(
			new UInt32[] { 0, 1, 2, 3 },
			new UInt32[] { 0, 0, 0, 0 })] UInt32[] key
			) {
			var messageAsBytes = MemoryMarshal.Cast<UInt64, byte>(message);
			var keyAsBytes = MemoryMarshal.Cast<UInt32, byte>(key);

			var legacyEncrypted = LegacyEncrypt(messageAsBytes, keyAsBytes);
			var newEncrypted = XTea.Encrypt(
				message: messageAsBytes,
				key: keyAsBytes).ToArray();

			Assert.That(newEncrypted, Is.EquivalentTo(legacyEncrypted));
		}


		/// <summary>
		/// Coz legacy code encrypts in-place.
		/// </summary>
		private static byte[] LegacyEncrypt(Span<byte> message, Span<byte> key) {
			var messageCopy = message.ToArray();
			var keyCopy = key.ToArray();
			var keyAsUInt32 = MemoryMarshal
				.Cast<byte, UInt32>(keyCopy)
				.ToArray();
			var length = messageCopy.Length;

			Security.Encryption.Xtea.Encrypt(
				buffer: ref messageCopy,
				length: ref length,
				index: 0,
				key: keyAsUInt32);

			return messageCopy;
		}
	}
}
