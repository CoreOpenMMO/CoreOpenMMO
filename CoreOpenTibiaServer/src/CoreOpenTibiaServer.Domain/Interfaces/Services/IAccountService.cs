using System.Threading.Tasks;
using COTS.Domain.Entities;

namespace COTS.Domain.Interfaces.Services
{
    public interface IAccountService : IBaseService<Account>
    {
        Task<bool> CheckAccountLogin(string username, string password);
    }
}
