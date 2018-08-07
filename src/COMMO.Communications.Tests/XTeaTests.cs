namespace COMMO.Communications.Tests {
	using COMMO.Communications.Criptography;
	using NUnit.Framework;
	using System;

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
	}
}
