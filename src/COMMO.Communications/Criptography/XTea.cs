namespace COMMO.Communications.Criptography {
	using System;
	using System.Runtime.InteropServices;

	public static class XTea {
		public const int MessageBlockSizeInBytes = 2 * sizeof(UInt32);
		public const int KeySizeInBytes = 4 * sizeof(UInt32);
		private const int DefaultOTCRoundCount = 32;

		private const uint BakedSum = 0xC6EF3720;
		private const uint Delta = 0x9E3779B9;

		public static Span<byte> Encrypt(ReadOnlySpan<byte> message, ReadOnlySpan<byte> key, int rounds = DefaultOTCRoundCount) {
			ThrowIfArgumentsAreInvalid(message, key);

			var clone = new byte[message.Length];
			message.CopyTo(clone);
			EncryptInplace(clone, key, rounds);
			return clone;
		}

		public static void EncryptInplace(Span<byte> message, ReadOnlySpan<byte> key, int rounds = DefaultOTCRoundCount) {
			ThrowIfArgumentsAreInvalid(message, key);

			var messageAsUInt32 = MemoryMarshal.Cast<byte, UInt32>(message);
			var keyAsUInt32 = MemoryMarshal.Cast<byte, UInt32>(key);
			for (int i = 0; i < messageAsUInt32.Length; i += 2) {
				var xSum = 0u;

				for (int j = 0; j < rounds; j++) {
					var temp = messageAsUInt32[i + 1];
					messageAsUInt32[i] += (((temp << 4) ^ (temp >> 5)) + temp) ^ (xSum + keyAsUInt32[(int)(xSum & 3)]);

					xSum += Delta;

					temp = messageAsUInt32[i];
					messageAsUInt32[i + 1] += (((temp << 4) ^ (temp >> 5)) + temp) ^ (xSum + keyAsUInt32[(int)((xSum >> 11) & 3)]);
				}
			}
		}

		public static Span<byte> Decrypt(ReadOnlySpan<byte> message, ReadOnlySpan<byte> key, int rounds = DefaultOTCRoundCount) {
			ThrowIfArgumentsAreInvalid(message, key);

			var clone = new byte[message.Length];
			message.CopyTo(clone);
			DecryptInplace(clone, key, rounds);
			return clone;
		}

		public static void DecryptInplace(Span<byte> message, ReadOnlySpan<byte> key, int rounds = DefaultOTCRoundCount) {
			ThrowIfArgumentsAreInvalid(message, key);

			var messageAsUInt32 = MemoryMarshal.Cast<byte, UInt32>(message);
			var keyAsUInt32 = MemoryMarshal.Cast<byte, UInt32>(key);
			for (int i = 0; i < messageAsUInt32.Length; i += 2) {
				var xSum = BakedSum;

				// Running decryption rounds
				for (int j = 0; j < rounds; j++) {
					var temp = messageAsUInt32[i];
					messageAsUInt32[i + 1] -= (((temp << 4) ^ (temp >> 5)) + temp) ^ (xSum + keyAsUInt32[(int)((xSum >> 11) & 3)]);

					xSum -= Delta;

					temp = messageAsUInt32[i + 1];
					messageAsUInt32[i] -= (((temp << 4) ^ (temp >> 5)) + temp) ^ (xSum + keyAsUInt32[(int)(xSum & 3)]);
				}
			}
		}

		private static void ThrowIfArgumentsAreInvalid(ReadOnlySpan<byte> message, ReadOnlySpan<byte> key) {
			if (message.Length < MessageBlockSizeInBytes)
				throw new ArgumentException(nameof(message) + $"'s size, in bytes, must be equal to or greater than {MessageBlockSizeInBytes}.");
			if (message.Length % MessageBlockSizeInBytes != 0)
				throw new ArgumentException(nameof(message) + $"'s size, in bytes, must be a multiple of {MessageBlockSizeInBytes}.");
			if (key.Length != KeySizeInBytes)
				throw new ArgumentException(nameof(key) + $"'s size, in bytes, must be exactly {KeySizeInBytes}.");
		}
	}
}
