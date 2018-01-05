using Newtonsoft.Json;
using System;
using System.Text;

namespace COTS.Infra.CrossCutting.Network {

    public sealed class NetworkMessage {
        public static readonly Encoding TextEncoder = Encoding.UTF8;

        public static string Decode(byte[] bytes) {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            return TextEncoder.GetString(bytes);
        }

        public static byte[] Encode(string message) {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return TextEncoder.GetBytes(message);
        }

        public static byte[] EncodeAndPrependByteCount(string message) {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var encodedMessageByteCount = TextEncoder.GetByteCount(message);
            // +4 because a int, used to store the message length, is guaranteed to be 4 bytes
            var buffer = new byte[4 + encodedMessageByteCount];

            // Copying the encodedMessageByteCount
            var encodedLength = BitConverter.GetBytes(encodedMessageByteCount);
            Array.Copy(encodedLength, buffer, 4);

            // Encoding the message
            var bytesWritten = TextEncoder.GetBytes(
               s: message,
               charIndex: 0,
               charCount: message.Length,
               bytes: buffer,
               byteIndex: 4);

            if (bytesWritten != encodedMessageByteCount)
                throw new InvalidOperationException("The Encoder lied to us >:(");

            return buffer;
        }

        public static byte[] EncodeAndPrependByteCount(LoginRequest loginInformation) {
            if (loginInformation == null)
                throw new ArgumentNullException(nameof(loginInformation));

            var serialized = JsonConvert.SerializeObject(loginInformation);
            var encoded = NetworkMessage.EncodeAndPrependByteCount(serialized);
            return encoded;
        }

        public static LoginRequest DecodeLoginInformation(byte[] bytes) {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            var decoded = NetworkMessage.Decode(bytes);
            var deserialized = JsonConvert.DeserializeObject<LoginRequest>(decoded);
            return deserialized;
        }
    }
}