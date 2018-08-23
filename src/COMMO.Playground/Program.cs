namespace COMMO.Playground {
	using System;
	using System.Linq;
	using NewRsa = COMMO.Communications.Criptography.RSA;
	using OldRsa = COMMO.Security.Encryption.Rsa;

	public static class Program {
		private static void Main(string[] args) {
			var originalData = new byte[] { 0, 1, 2, 3, 4, 5 };
			SanityCheck(originalData);

			originalData = new byte[] { 0, 1, 2, 3, 4, 5 };
			NewEncrypt_OldDecrypt(originalData);

			originalData = new byte[] { 0, 1, 2, 3, 4, 5 };
			NewDecrypt_OldEncrypt(originalData);
		}

		private static void NewDecrypt_OldEncrypt(byte[] originalData) {
			var buffer = originalData.ToArray();
			Array.Resize(ref buffer, 128);
			OldRsa.Encrypt(buffer: ref buffer, position: 0, useCipValues: false);

			var decrypted = NewRsa.DecryptWithOTCKeys(buffer);
			Console.WriteLine($"NewDecrypt OldEncrypt = {originalData.SequenceEqual(decrypted)}");
		}

		private static void NewEncrypt_OldDecrypt(byte[] originalData) {
			var newData = originalData.ToArray();
			var buffer = NewRsa.EncryptWithOTCKeys(newData);
			OldRsa.Decrypt(
				buffer: ref buffer,
				position: 0,
				length: buffer.Length,
				useCipValues: false);
			Array.Resize(ref buffer, originalData.Length);
			Console.WriteLine($"New Encrypt and Old Decrypt == {originalData.SequenceEqual(buffer)}");
			Console.WriteLine();

			buffer = NewRsa.EncryptWithOTCKeys(newData);
			Array.Reverse(buffer);
			OldRsa.Decrypt(
				buffer: ref buffer,
				position: 0,
				length: buffer.Length,
				useCipValues: false);
			Array.Resize(ref buffer, originalData.Length);
			Console.WriteLine($"New Encrypt and Old Decrypt == {originalData.SequenceEqual(buffer)}");
			Console.WriteLine();

			buffer = NewRsa.EncryptWithOTCKeys(newData);
			Array.Resize(ref buffer, newData.Length);
			Array.Resize(ref buffer, 128);
			OldRsa.Decrypt(
				buffer: ref buffer,
				position: 0,
				length: buffer.Length,
				useCipValues: false);
			Array.Resize(ref buffer, originalData.Length);
			Console.WriteLine($"New Encrypt and Old Decrypt == {originalData.SequenceEqual(buffer)}");
			Console.WriteLine();


			buffer = NewRsa.EncryptWithOTCKeys(newData);
			Array.Resize(ref buffer, newData.Length);
			Array.Reverse(buffer);
			Array.Resize(ref buffer, 128);
			OldRsa.Decrypt(
				buffer: ref buffer,
				position: 0,
				length: buffer.Length,
				useCipValues: false);
			Array.Resize(ref buffer, originalData.Length);
			Console.WriteLine($"New Encrypt and Old Decrypt == {originalData.SequenceEqual(buffer)}");
			Console.WriteLine();

			buffer = NewRsa.EncryptWithOTCKeys(newData);
			buffer = buffer.Skip(128 - newData.Length).ToArray();
			Array.Resize(ref buffer, 128);
			OldRsa.Decrypt(
				buffer: ref buffer,
				position: 0,
				length: buffer.Length,
				useCipValues: false);
			Array.Resize(ref buffer, originalData.Length);
			Console.WriteLine($"New Encrypt and Old Decrypt == {originalData.SequenceEqual(buffer)}");
			Console.WriteLine();

			buffer = NewRsa.EncryptWithOTCKeys(newData);
			buffer = buffer.Skip(128 - newData.Length).ToArray();
			Array.Reverse(buffer);
			Array.Resize(ref buffer, 128);
			OldRsa.Decrypt(
				buffer: ref buffer,
				position: 0,
				length: buffer.Length,
				useCipValues: false);
			Array.Resize(ref buffer, originalData.Length);
			Console.WriteLine($"New Encrypt and Old Decrypt == {originalData.SequenceEqual(buffer)}");
			Console.WriteLine();
		}

		private static void SanityCheck(Byte[] originalData) {
			var newData = originalData.ToArray();
			var newEncrypted = NewRsa.EncryptWithOTCKeys(newData);
			var newDecrypted = NewRsa.DecryptWithOTCKeys(newEncrypted);
			Console.WriteLine($"New Encrypt and Decrypt == {originalData.SequenceEqual(newDecrypted)}");

			var oldBuffer = originalData.ToArray();
			Array.Resize(ref oldBuffer, 128);
			var oldEncrypted = OldRsa.Encrypt(
				buffer: ref oldBuffer,
				position: 0,
				useCipValues: false);

			OldRsa.Decrypt(
				buffer: ref oldBuffer,
				position: 0,
				length: oldBuffer.Length,
				useCipValues: false);
			Array.Resize(ref oldBuffer, originalData.Length);

			Console.WriteLine($"Old Encrypt and Decrypt == {originalData.SequenceEqual(oldBuffer)}");
		}
	}
}
