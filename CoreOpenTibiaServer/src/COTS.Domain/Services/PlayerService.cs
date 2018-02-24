using COTS.Domain.Entities;
using COTS.Domain.Interfaces.Repositories;
using COTS.Domain.Interfaces.Services;

namespace COTS.Domain.Services
{
    public class PlayerService : BaseService<Player>, IPlayerService
    {
        public PlayerService(IPlayerRepository playerRepository) : base(playerRepository)
        {
        }
    }
}
