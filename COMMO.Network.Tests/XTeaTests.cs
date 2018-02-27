using COMMO.Network.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace COMMO.Network.Tests {

	[TestClass]
	public sealed class XTeaTests {
		private readonly uint[] _referenceEncryptionKey = new uint[] { 3442030272, 2364789040, 1503299581, 3670909886 };
		private readonly byte[] _referenceMessageToEncrypt = new byte[] { 123, 0, 20, 20, 0, 48, 10, 66, 101, 109, 32, 118, 105, 110, 100, 111 };
		private readonly byte[] _referenceEncryptedMessage = new byte[] { 163, 35, 242, 102, 150, 173, 252, 174, 83, 54, 209, 248, 35, 145, 205, 229 };

		[TestMethod]
		public void XTeaEncryption_Validation() {
			var encryptedMessage = XTea.EncryptXtea(_referenceMessageToEncrypt, _referenceEncryptionKey);
			CollectionAssert.AreEqual(encryptedMessage, _referenceEncryptedMessage);
		}

		[TestMethod]
		public void XTeaDecryption_Validation() {
			var decryptedMsg = XTea.DecryptXtea(_referenceEncryptedMessage, _referenceEncryptionKey);
			CollectionAssert.AreEqual(decryptedMsg, _referenceMessageToEncrypt);
		}

		[TestMethod]
		[ExpectedException(typeof(System.Exception))]
		public void XTeaEncryption_Validation_EmptyMessage() {
			var encryptedMessage = XTea.EncryptXtea(new byte[] {}, _referenceEncryptionKey);
			CollectionAssert.AreEqual(encryptedMessage, _referenceEncryptedMessage);
		}

		[TestMethod]
		[ExpectedException(typeof(System.Exception))]
		public void XTeaEncryption_Validation_WrongBlockSize() {
			var encryptedMessage = XTea.EncryptXtea(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, _referenceEncryptionKey);
			CollectionAssert.AreEqual(encryptedMessage, _referenceEncryptedMessage);
		}

		[TestMethod]
		[ExpectedException(typeof(System.Exception))]
		public void XTeaEncryption_Validation_WrongKeySize() {
			var encryptedMessage = XTea.EncryptXtea(_referenceEncryptedMessage, new uint[] { 0, 0, 0 });
			CollectionAssert.AreEqual(encryptedMessage, _referenceEncryptedMessage);
		}
	}
}