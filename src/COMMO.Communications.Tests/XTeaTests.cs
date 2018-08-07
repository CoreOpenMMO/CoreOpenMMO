namespace COMMO.Communications.Tests {
	using System;
	using NUnit.Framework;
	using COMMO.Communications.Criptography;

	[TestFixture]
	public sealed class XTeaTests {

		[Test]
		public void InvalidKeySize() {
			var message = new byte[] { 0, 0, 0, 0, 0, 0 };
			var key = new byte[] { };

			Assert.Throws<ArgumentException>(() => XTea.Encrypt(message: message, key: key));
		}
	}
}
