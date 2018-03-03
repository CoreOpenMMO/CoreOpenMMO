using System;
using System.Linq;

namespace COMMO.GameServer {

	public sealed class Program {

		private static void Main(string[] args) {

			var message = new Span<byte>(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
			var key = new Span<byte>(new byte[] { 1, 2, 3, 4 });

			var crypto = Network.Cryptography.PseudoXTea.Encrypt(message, key);
			var decrypto = Network.Cryptography.PseudoXTea.Decrypt(crypto, key);

			Console.WriteLine(Enumerable.SequenceEqual(message.ToArray(), decrypto.ToArray()));

			for (int i = 0; i < message.Length; i++)
				Console.Write(message[i] + " ");
			Console.WriteLine();

			for (int i = 0; i < decrypto.Length; i++)
				Console.Write(decrypto[i] + " ");
			Console.WriteLine();
		}
	}
}
