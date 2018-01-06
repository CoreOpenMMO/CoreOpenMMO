using System.Threading.Tasks;
using COTS.Domain.Entities;

namespace COTS.Domain.Interfaces.Repositories
{
    public interface IAccountRepository : IRepositoryBase<Account>
    {
        Task<Account> GetAccountByLogin(string username, string password);
    }
}
