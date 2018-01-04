using System;
using System.Text;

namespace COTS.Infra.CrossCutting.Network {

    public sealed class NetworkMessage {
        public static readonly Encoding TextEncoding = Encoding.UTF8;
        public static readonly string ContentSeparator = Environment.NewLine;

        public static string Decode(byte[] bytes) {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            return TextEncoding.GetString(bytes);
        }

        public static byte[] Encode(string message) {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return TextEncoding.GetBytes(message);
        }
    }
}