using System;

namespace COMMO.Network.Cryptography {

	public static class PseudoXTea {
		private const int DefaultOTCRoundCount = 32;
		private const int SupportedMessageBlockLength = 8;
		private const int SupportedKeyLength = 4;

		private const uint BakedSum = 0xC6EF3720; //(Delta * Rounds)
		private const uint Delta = 0x9E3779B9;
		private const uint Rounds = 32;

		public static Span<byte> Encrypt(ReadOnlySpan<byte> message, ReadOnlySpan<byte> key) {
			ThrowIfArgumentsAreInvalid(message, key);

			var clone = new Span<byte>(new byte[message.Length]);
			message.CopyTo(clone);
			EncryptInplace(clone, key);
			return clone;
		}

		public static void EncryptInplace(Span<byte> message, ReadOnlySpan<byte> key) {
			ThrowIfArgumentsAreInvalid(message, key);

			var messageAsUIntArray = message.NonPortableCast<byte, uint>();
			for (int i = 0; i < messageAsUIntArray.Length; i += 2) {
				var xSum = 0u;

				for (int rounds = 0; rounds < DefaultOTCRoundCount; rounds++) {
					var temp = messageAsUIntArray[i + 1];
					messageAsUIntArray[i] += (((temp << 4) ^ (temp >> 5)) + temp) ^ (xSum + key[(int)(xSum & 3)]);

					xSum += Delta;

					temp = messageAsUIntArray[i];
					messageAsUIntArray[i + 1] += (((temp << 4) ^ (temp >> 5)) + temp) ^ (xSum + key[(int)((xSum >> 11) & 3)]);
				}
			}
		}

		public static Span<byte> Decrypt(ReadOnlySpan<byte> message, ReadOnlySpan<byte> key) {
			ThrowIfArgumentsAreInvalid(message, key);

			var clone = new Span<byte>(new byte[message.Length]);
			message.CopyTo(clone);
			DecryptInplace(clone, key);
			return clone;
		}

		public static void DecryptInplace(Span<byte> message, ReadOnlySpan<byte> key) {
			ThrowIfArgumentsAreInvalid(message, key);

			var messageAsUIntArray = message.NonPortableCast<byte, uint>();
			for (int i = 0; i < messageAsUIntArray.Length; i += 2) {
				var xSum = BakedSum;

				for (int rounds = 0; rounds < DefaultOTCRoundCount; rounds++) {
					var temp = messageAsUIntArray[i];
					messageAsUIntArray[i + 1] -= (((temp << 4) ^ (temp >> 5)) + temp) ^ (xSum + key[(int)((xSum >> 11) & 3)]);

					xSum -= Delta;

					temp = messageAsUIntArray[i + 1];
					messageAsUIntArray[i] -= (((temp << 4) ^ (temp >> 5)) + temp) ^ (xSum + key[(int)(xSum & 3)]);
				}
			}
		}

		private static void ThrowIfArgumentsAreInvalid(ReadOnlySpan<byte> message, ReadOnlySpan<byte> key) {
			if (message.Length < SupportedMessageBlockLength)
				throw new ArgumentException(nameof(message) + $"'s length must be equal to or greater than {SupportedMessageBlockLength}.");
			if (message.Length % SupportedMessageBlockLength != 0)
				throw new ArgumentException(nameof(message) + $"'s length must be a multiple of {SupportedMessageBlockLength}.");
			if (key.Length != SupportedKeyLength)
				throw new ArgumentOutOfRangeException(nameof(key) + $"'s length must be exactly {SupportedKeyLength}.");
		}
	}
}

