namespace COTS.Domain.Services
{
    using Entities;
    using Interfaces.Repositories;
    using Interfaces.Services;

    public class AccountService : BaseService<Account>, IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository) 
            : base(accountRepository) 
                => _accountRepository = accountRepository;

        public Account GetAccountByLogin(string username, string password) =>
            _accountRepository.GetAccountByLogin(username, password).Result;
    }
}
