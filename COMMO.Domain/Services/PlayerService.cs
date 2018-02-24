using System.Collections.Generic;

namespace COMMO.Domain.Services
{
    using Domain.Entities;
    using Domain.Interfaces.Repositories;
    using Domain.Interfaces.Services;

    public class PlayerService : BaseService<Player>, IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;

		public PlayerService(IPlayerRepository playerRepository)
			: base(playerRepository) {
			_playerRepository = playerRepository;
		}

		public IEnumerable<string> GetCharactersListByAccountId(int id) =>
            _playerRepository.GetCharactersListByAccountId(id);
    }
}
