using System.Threading.Tasks;

namespace COMMO.Domain.Interfaces.Repositories
{
    using Entities;

    public interface IAccountRepository : IRepositoryBase<Account>
    {
        Task<Account> GetAccountByLogin(string username, string password);
    }
}
