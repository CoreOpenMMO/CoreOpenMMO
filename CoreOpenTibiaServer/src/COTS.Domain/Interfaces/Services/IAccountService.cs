using COTS.Domain.Entities;

namespace COTS.Domain.Interfaces.Services
{
    public interface IAccountService : IBaseService<Account>
    {
        Account GetAccountByLogin(string username, string password);
    }
}
