using System.Collections.Generic;
using COTS.Domain.Entities;

namespace COTS.Domain.Interfaces.Services
{
    public interface IPlayerService : IBaseService<Player>
    {
        List<string> GetCharactersListByAccountId(int id);
    }
}
