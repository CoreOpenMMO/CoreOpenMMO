namespace COMMO.Playground {
	using System;
	using System.Linq;
	using NewRsa = COMMO.Communications.Criptography.OTClientRSA;
	using OldRsa2 = COMMO.Security.Encryption.Rsa2;

	public static class Program {
		private static void Main(string[] args) {
			var originalData = new byte[] { 7, 1, 2, 3, 4, 5 };

			var buffer = NewRsa.Encrypt(originalData).ToArray();
			OldRsa2.Decrypt(
				buffer: ref buffer,
				index: 0,
				lenght: originalData.Length);

			buffer = buffer
				.Skip(128 - originalData.Length)
				.ToArray();			

			Console.WriteLine(originalData.SequenceEqual(buffer));			
			// OldRsa2Encrypt_NewDecrypt(originalData);
		}

		private static void OldRsa2Encrypt_NewDecrypt(Byte[] originalData) {
			var buffer = originalData.ToArray();
			Array.Resize(ref buffer, 128);

			OldRsa2.Encrypt(
				buffer: ref buffer,
				0);

			buffer = NewRsa
				.Decrypt(buffer)
				.ToArray()
				.Take(originalData.Length)
				.ToArray();

			Console.WriteLine(originalData.SequenceEqual(buffer));
		}
	}
}
