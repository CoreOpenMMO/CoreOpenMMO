using System;

namespace COTS.Infra.CrossCutting.Network {

    public sealed class LoginInformation {
        public readonly string Account;
        public readonly string Password;

        public LoginInformation(string account, string password) {
            if (account == null)
                throw new ArgumentNullException(nameof(account));
            if (password == null)
                throw new ArgumentNullException(nameof(account));
            Account = account;
            Password = password;
        }
    }
}