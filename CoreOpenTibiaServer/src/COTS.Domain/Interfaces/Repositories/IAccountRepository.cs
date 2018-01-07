using System.Threading.Tasks;

namespace COTS.Domain.Interfaces.Repositories
{
    using Entities;

    public interface IAccountRepository : IRepositoryBase<Account>
    {
        Task<Account> GetAccountByLogin(string username, string password);
    }
}
