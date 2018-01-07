using Newtonsoft.Json;
using System;
using System.Text;

namespace COTS.Infra.CrossCutting.Network
{
    public sealed class NetworkMessage
    {
        public static Encoding TextEncoder => Encoding.UTF8;

        public static string Decode(byte[] bytes) =>
            TextEncoder.GetString(bytes
                ?? throw new ArgumentNullException(nameof(bytes)));

        public static byte[] Encode(string message) => 
            TextEncoder.GetBytes(message 
                ?? throw new ArgumentNullException(nameof(message)));

        public static byte[] EncodeAndPrependByteCount(string message)
        {
            var encodedMessageByteCount = TextEncoder.GetByteCount(message
                ?? throw new ArgumentNullException(nameof(message)));

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

        public static byte[] EncodeAndPrependByteCount(LoginRequest loginRequest) =>
            EncodeAndPrependByteCount(JsonConvert.SerializeObject(loginRequest
                ?? throw new ArgumentNullException(nameof(loginRequest))));

        public static LoginRequest DecodeLoginRequest(byte[] bytes) => 
            JsonConvert.DeserializeObject<LoginRequest>(Decode(bytes)
                ?? throw new ArgumentNullException(nameof(bytes)));

        public static byte[] EncodeAndPrependByteCount(LoginResponse loginResponse) => 
            EncodeAndPrependByteCount(JsonConvert.SerializeObject(loginResponse 
                ?? throw new ArgumentNullException(nameof(loginResponse))));

        public static LoginResponse DecodeLoginResponse(byte[] bytes) => 
            JsonConvert.DeserializeObject<LoginResponse>(Decode(bytes
                ?? throw new ArgumentNullException(nameof(bytes))));
    }
}