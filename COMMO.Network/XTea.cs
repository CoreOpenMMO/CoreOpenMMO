using System;

namespace COMMO.Network {

	public static class XTea {
		private const int DefaultOTCRoundCount = 32;
		private const int MessageBlockLength = 8;
		private const int KeySizeInBytes = 4;

		private const uint BakedSum = 0xC6EF3720;
		private const uint Delta = 0x9E3779B9;

		public static Span<byte> Encrypt(ReadOnlySpan<byte> message, ReadOnlySpan<uint> key, int rounds = DefaultOTCRoundCount) {
			ThrowIfArgumentsAreInvalid(message, key);

			var clone = new Span<byte>(new byte[message.Length]);
			message.CopyTo(clone);
			EncryptInplace(clone, key, rounds);
			return clone;
		}

		public static void EncryptInplace(Span<byte> message, ReadOnlySpan<uint> key, int rounds = DefaultOTCRoundCount) {
			ThrowIfArgumentsAreInvalid(message, key);

			var messageAsUIntArray = message.NonPortableCast<byte, uint>();
			for (int i = 0; i < messageAsUIntArray.Length; i += 2) {
				var xSum = 0u;

				for (int j = 0; j < rounds; j++) {
					var temp = messageAsUIntArray[i + 1];
					messageAsUIntArray[i] += (((temp << 4) ^ (temp >> 5)) + temp) ^ (xSum + key[(int)(xSum & 3)]);

					xSum += Delta;

					temp = messageAsUIntArray[i];
					messageAsUIntArray[i + 1] += (((temp << 4) ^ (temp >> 5)) + temp) ^ (xSum + key[(int)((xSum >> 11) & 3)]);
				}
			}
		}

		public static Span<byte> Decrypt(ReadOnlySpan<byte> message, ReadOnlySpan<uint> key, int rounds = DefaultOTCRoundCount) {
			ThrowIfArgumentsAreInvalid(message, key);

			var clone = new Span<byte>(new byte[message.Length]);
			message.CopyTo(clone);
			DecryptInplace(clone, key, rounds);
			return clone;
		}

		public static void DecryptInplace(Span<byte> message, ReadOnlySpan<uint> key, int rounds = DefaultOTCRoundCount) {
			ThrowIfArgumentsAreInvalid(message, key);

			var messageAsUIntArray = message.NonPortableCast<byte, uint>();
			for (int i = 0; i < messageAsUIntArray.Length; i += 2) {
				var xSum = BakedSum;

				// Running decryption rounds
				for (int j = 0; j < rounds; j++) {
					var temp = messageAsUIntArray[i];
					messageAsUIntArray[i + 1] -= (((temp << 4) ^ (temp >> 5)) + temp) ^ (xSum + key[(int)((xSum >> 11) & 3)]);

					xSum -= Delta;

					temp = messageAsUIntArray[i + 1];
					messageAsUIntArray[i] -= (((temp << 4) ^ (temp >> 5)) + temp) ^ (xSum + key[(int)(xSum & 3)]);
				}
			}
		}

		private static void ThrowIfArgumentsAreInvalid(ReadOnlySpan<byte> message, ReadOnlySpan<uint> key) {
			if (message.Length < MessageBlockLength)
				throw new ArgumentException(nameof(message) + $"'s length must be equal to or greater than {MessageBlockLength}.");
			if (message.Length % MessageBlockLength != 0)
				throw new ArgumentException(nameof(message) + $"'s length must be a multiple of {MessageBlockLength}.");
			if (key.Length != KeySizeInBytes)
				throw new ArgumentOutOfRangeException(nameof(key) + $"'s length must be exactly {KeySizeInBytes}.");
		}
	}
}

