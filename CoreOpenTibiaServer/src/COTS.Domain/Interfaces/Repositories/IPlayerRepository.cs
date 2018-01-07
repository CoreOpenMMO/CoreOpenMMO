using System.Collections.Generic;

namespace COTS.Domain.Interfaces.Repositories
{
    using Entities;

    public interface IPlayerRepository : IRepositoryBase<Player>
    {
        IEnumerable<string> GetCharactersListByAccountId(int id);
    }
}
