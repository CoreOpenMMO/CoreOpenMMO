using System;
using MoonSharp.Interpreter;

namespace COTS.Domain.LuaServices
{
    using Domain.Entities;
    using Domain.Interfaces.Repositories;
    using System.Threading.Tasks;

    [MoonSharpUserData]
    public class LuaPlayerService
    {
        private static IPlayerRepository _playerRepository;

        public LuaPlayerService(IPlayerRepository playerRepository) =>
            _playerRepository = playerRepository;

        public virtual async Task<Player> GetByGuid(Guid guid) =>
            await _playerRepository.GetByGuid(guid);

        public virtual async Task<Player> GetById(int id) =>
            await _playerRepository.GetById(id);

        public virtual async Task<string> GetNameById(int id) =>
            (await _playerRepository.GetById(id)).Name;
    }
}
