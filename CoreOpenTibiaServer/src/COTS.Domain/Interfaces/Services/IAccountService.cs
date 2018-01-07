using COTS.Domain.Entities;

namespace COTS.Domain.Interfaces.Services
{
    public interface IAccountService : IServiceBase<Account>
    {
        Account GetAccountByLogin(string username, string password);
    }
}
