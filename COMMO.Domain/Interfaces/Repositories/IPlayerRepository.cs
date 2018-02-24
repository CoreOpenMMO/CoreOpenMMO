using System.Collections.Generic;

namespace COMMO.Domain.Interfaces.Repositories
{
    using Entities;

    public interface IPlayerRepository : IRepositoryBase<Player>
    {
        IEnumerable<string> GetCharactersListByAccountId(int id);
    }
}
