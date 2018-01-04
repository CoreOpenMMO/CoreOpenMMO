using System.Threading.Tasks;
using COTS.Domain.Entities;

namespace COTS.Domain.Interfaces.Repositories
{
    public interface IAccountRepository : IRepositoryBase<Account>
    {
        Task<bool> CheckAccountLogin(string username, string password);
    }
}
