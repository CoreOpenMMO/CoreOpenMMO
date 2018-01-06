using System.Collections.Generic;
using System.Threading.Tasks;
using COTS.Domain.Entities;

namespace COTS.Domain.Interfaces.Repositories
{
    public interface IPlayerRepository : IRepositoryBase<Player>
    {
        Task<List<string>> GetCharactersListByAccountId(int id);
    }
}
