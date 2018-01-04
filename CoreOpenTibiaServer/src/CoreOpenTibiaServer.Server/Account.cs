using System;
using System.Collections.Generic;
using System.Linq;

namespace COTS.Server {

    /// <summary>
    /// This class is immutable because it's only created by the data base manager.
    /// During the server execution, a account is never modified.
    /// </summary>
    public sealed class Account : IEquatable<Account> {

        public enum AccountType {
            Normal,
            Tutor,
            SeniorTutor,
            GameMaster,
            God
        }

        public readonly AccountType Type;
        public readonly string Name;
        public readonly string Password;

        public readonly DateTime PremiumAccountStart;
        public readonly TimeSpan PremiumAccountDuration;
        public DateTime PremiumAccountEnd => PremiumAccountStart.Add(PremiumAccountDuration);

        public readonly IReadOnlyList<Character> Characters;

        public Account(
            string name,
            string password,
            AccountType type,
            DateTime premiumAccountStart,
            TimeSpan premiumAccountDuration,
            IEnumerable<Character> characters
            ) {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (password == null)
                throw new ArgumentNullException(nameof(password));
            if (characters == null)
                throw new ArgumentNullException(nameof(characters));

            this.Name = name;
            this.Password = password;
            this.Type = type;
            this.PremiumAccountStart = premiumAccountStart;
            this.PremiumAccountDuration = premiumAccountDuration;
            this.Characters = characters.ToArray();
        }

        public bool IsPremium() {
            var now = DateTime.UtcNow;
            return PremiumAccountStart <= now && now <= PremiumAccountEnd;
        }

        public override bool Equals(object obj) => Equals(obj as Account);

        public bool Equals(Account other) {
            return other != null &&
                Name.Equals(other.Name) &&
                Password.Equals(other.Password) &&
                Type.Equals(other.Type) &&
                PremiumAccountStart.Equals(other.PremiumAccountStart) &&
                PremiumAccountDuration.Equals(other.PremiumAccountDuration) &&
                Enumerable.SequenceEqual(Characters, other.Characters);
        }

        public override int GetHashCode() {
            return HashHelper.Start
                .CombineHashCode(Name)
                .CombineHashCode(Password);
        }
    }
}