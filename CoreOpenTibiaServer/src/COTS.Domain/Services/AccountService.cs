﻿using COTS.Domain.Entities;
using COTS.Domain.Interfaces.Repositories;
using COTS.Domain.Interfaces.Services;

namespace COTS.Domain.Services
{
    public class AccountService : BaseService<Account>, IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository) : base (accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Account GetAccountByLogin(string username, string password)
        {
            return _accountRepository.GetAccountByLogin(username, password).Result;
        }
    }
}
