using System;
using MoonSharp.Interpreter;

namespace COMMO.Domain.LuaServices
{
    using Domain.Entities;
    using Domain.Interfaces.Repositories;
    using System.Threading.Tasks;

    [MoonSharpUserData]
    public class LuaPlayerService
    {
        private static IPlayerRepository _playerRepository;

		public LuaPlayerService(IPlayerRepository playerRepository) {
			_playerRepository = playerRepository;
		}

		public virtual Player GetByGuid(Guid guid) =>
            _playerRepository.GetByGuid(guid).Result;

        public virtual Player GetById(int id) =>
            _playerRepository.GetById(id).Result;

        public virtual string GetNameById(int id) =>
            _playerRepository.GetById(id).Result.Name;
    }
}
