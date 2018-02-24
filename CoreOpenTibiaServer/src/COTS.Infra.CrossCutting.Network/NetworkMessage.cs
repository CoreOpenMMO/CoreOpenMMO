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

            // To anyone trying to implement our protocol, it's important to notice
            // that sizeof(int) is guaranteed to be 4.
            // We are using sizeof just to avoid magic numbers.
            var buffer = new byte[sizeof(int) + encodedMessageByteCount];

            // Copying the encodedMessageByteCount
            var encodedLength = BitConverter.GetBytes(encodedMessageByteCount);
            Array.Copy(encodedLength, buffer, sizeof(int));

            // Encoding the message
            var bytesWritten = TextEncoder.GetBytes(
               s: message,
               charIndex: 0,
               charCount: message.Length,
               bytes: buffer,
               byteIndex: sizeof(int));

            if (bytesWritten != encodedMessageByteCount)
                throw new InvalidOperationException("The Encoder lied to us >:(");

            return buffer;
        }

        public static byte[] EncodeAndPrependByteCount(LoginRequest loginRequest) {
            if (loginRequest == null)
                throw new ArgumentNullException(nameof(loginRequest));

            var serialized = JsonConvert.SerializeObject(loginRequest);
            var encoded = NetworkMessage.EncodeAndPrependByteCount(serialized);
            return encoded;
        }

        public static LoginRequest DecodeLoginRequest(byte[] bytes) {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            var decoded = NetworkMessage.Decode(bytes);
            var deserialized = JsonConvert.DeserializeObject<LoginRequest>(decoded);
            return deserialized;
        }

        public static byte[] EncodeAndPrependByteCount(LoginResponse loginResponse) {
            if (loginResponse == null)
                throw new ArgumentNullException(nameof(loginResponse));

            var serialized = JsonConvert.SerializeObject(loginResponse);
            var encoded = NetworkMessage.EncodeAndPrependByteCount(serialized);
            return encoded;
        }

        public static LoginResponse DecodeLoginResponse(byte[] bytes) {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            var decoded = NetworkMessage.Decode(bytes);
            var deserialized = JsonConvert.DeserializeObject<LoginResponse>(decoded);
            return deserialized;
        }
    }
}