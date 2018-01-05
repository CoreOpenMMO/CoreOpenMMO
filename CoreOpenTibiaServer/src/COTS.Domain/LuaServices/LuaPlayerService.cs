using System;
using COTS.Domain.Entities;
using COTS.Domain.Interfaces.Repositories;
using MoonSharp.Interpreter;

namespace COTS.Domain.LuaServices
{
    [MoonSharpUserData]
    public class LuaPlayerService
    {
        private static IPlayerRepository _playerRepository;

        public LuaPlayerService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public static Player GetByGuid(Guid guid)
        {
            return _playerRepository.GetByGuid(guid).Result;
        }
        
        public static Player GetById(int id)
        {
            return _playerRepository.GetById(id).Result;
        }

        public static string GetNameById(int id)
        {
            return _playerRepository.GetById(id).Result.Name;
        }
    }
}
