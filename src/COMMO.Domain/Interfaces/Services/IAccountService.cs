using COMMO.Domain.Entities;

namespace COMMO.Domain.Interfaces.Services
{
    public interface IAccountService : IServiceBase<Account>
    {
        Account GetAccountByLogin(string username, string password);
    }
}
