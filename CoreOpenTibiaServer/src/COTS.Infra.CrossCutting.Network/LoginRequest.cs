using Newtonsoft.Json;
using System;

namespace COTS.Infra.CrossCutting.Network {

    [JsonObject(MemberSerialization.OptOut)]
    public sealed class LoginRequest : IEquatable<LoginRequest> {

        [JsonProperty(Order = 0)]
        public readonly int ClientVersion;

        [JsonProperty(Order = 1)]
        public readonly int CommunicationProtocolVersion;

        [JsonProperty(Order = 2)]
        public readonly string Account;

        [JsonProperty(Order = 3)]
        public readonly string Password;

        [JsonConstructor]
        public LoginRequest(
            int clientVersion,
            int communicationProtocolVersion,
            string account,
            string password
            ) {
            if (account == null)
                throw new ArgumentNullException(nameof(account));
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            ClientVersion = clientVersion;
            CommunicationProtocolVersion = communicationProtocolVersion;
            Account = account;
            Password = password;
        }

        public override int GetHashCode() {
            return HashHelper.Start
                .CombineHashCode(ClientVersion)
                .CombineHashCode(CommunicationProtocolVersion)
                .CombineHashCode(Account)
                .CombineHashCode(Password);
        }

        public override bool Equals(object obj) => Equals(obj as LoginRequest);

        public bool Equals(LoginRequest other) {
            return other != null &&
                ClientVersion == other.ClientVersion &&
                CommunicationProtocolVersion == other.CommunicationProtocolVersion &&
                Account == other.Account &&
                Password == other.Password;
        }
    }
}