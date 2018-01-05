using Newtonsoft.Json;
using System;

namespace COTS.Infra.CrossCutting.Network {

    [JsonObject(MemberSerialization.OptOut)]
    public sealed class LoginRequest {

        [JsonProperty(Order = 0)]
        public readonly int ClientVersion;

        [JsonProperty(Order = 1)]
        public readonly int ProtocolVersion;

        [JsonProperty(Order = 2)]
        public readonly string Account;

        [JsonProperty(Order = 3)]
        public readonly string Password;

        [JsonConstructor]
        public LoginRequest(
            int clientVersion,
            int protocolVersion,
            string account,
            string password
            ) {
            if (account == null)
                throw new ArgumentNullException(nameof(account));
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            ClientVersion = clientVersion;
            ProtocolVersion = protocolVersion;
            Account = account;
            Password = password;
        }
    }
}