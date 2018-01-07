using System.Collections.Generic;
using COTS.Domain.Entities;

namespace COTS.Domain.Interfaces.Services
{
    public interface IPlayerService : IServiceBase<Player>
    {
        List<string> GetCharactersListByAccountId(int id);
    }
}
