using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace COTS.Infra.CrossCutting.Network
{
    [JsonObject(MemberSerialization.OptOut)]
    public sealed class LoginResponse : IEquatable<LoginResponse>
    {
        public enum ResponseType
        {
            FailedDueToMalformedLoginRequest = 0,
            DeniedDueToIncompatibleProtocolVersion = 1,
            DeniedDueToIncompatibleClientVersion = 2,
            Sucess = 3,
            AccountBanned = 4,
            IpBanned = 5
        }

        [JsonProperty(Order = 0)]
        public readonly ResponseType Status;

        [JsonProperty(Order = 1)]
        public readonly int PremiumAccountDaysRemaining;

        [JsonProperty(Order = 2)]
        public readonly IReadOnlyList<string> CharacterNames;

        [JsonConstructor]
        public LoginResponse(
            ResponseType status,
            IEnumerable<string> characterNames,
            int premiumAccountDaysRemaining)
        {
            Status = status;
            PremiumAccountDaysRemaining = premiumAccountDaysRemaining;
            CharacterNames = characterNames?.ToArray() 
                ?? throw new ArgumentNullException(nameof(characterNames));
        }

        public override int GetHashCode() =>
            HashHelper.Start
                .CombineHashCode(Status)
                .CombineHashCode(CharacterNames.Count)
                .CombineHashCode(PremiumAccountDaysRemaining);

        public override bool Equals(object obj) => 
            Equals(obj as LoginResponse);

        public bool Equals(LoginResponse other) => 
            other != null &&
            Status == other.Status &&
            PremiumAccountDaysRemaining == other.PremiumAccountDaysRemaining &&
            Enumerable.SequenceEqual(CharacterNames, other.CharacterNames);
    }
}