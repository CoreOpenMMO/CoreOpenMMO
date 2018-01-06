using System.Collections.Generic;
using COTS.Domain.Entities;
using COTS.Domain.Interfaces.Repositories;
using COTS.Domain.Interfaces.Services;

namespace COTS.Domain.Services
{
    public class PlayerService : BaseService<Player>, IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerService(IPlayerRepository playerRepository) : base(playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public List<string> GetCharactersListByAccountId(int id)
        {
            return _playerRepository.GetCharactersListByAccountId(id).Result;
        }
    }
}
