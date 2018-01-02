using MoonSharp.Interpreter;
using SharpServer.Database;
using SharpServer.Domain.Entities;

namespace SharpServer.Server.LuaServices
{
    [MoonSharpUserData]
    public class LuaPlayerService
    {
        private static PlayerRepository _playerRepository;

        public LuaPlayerService()
        {
            _playerRepository = new PlayerRepository();
        }

        public static Player GetPlayerByGuid(int guid)
        {
            return _playerRepository.GetPlayerByGuid(guid);
        }

        //TODO delete this method
        public static string GetPlayerNameByGuid(int guid)
        {
            return _playerRepository.GetPlayerNameByGuid(guid);
        }
    }
}
