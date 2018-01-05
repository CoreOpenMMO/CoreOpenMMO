using System;
using COTS.Domain.LuaServices;
using Microsoft.Extensions.Logging;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace COTS.GameServer.Lua
{
    public class LuaManager
    {
        private static ILogger<LuaManager> _logger;
        private static IServiceProvider _provider;
        private static Script _script;

        public LuaManager(ILogger<LuaManager> logger, IServiceProvider provider)
        {
            _provider = provider;
            _logger = logger;
        }
        
        public void Run()
        {
            _script = new Script();

            LoadLuaFiles();
            RegisterLuaServices();
        }

        private void LoadLuaFiles()
        {
            Console.WriteLine("Loading lua files...");
            _logger.LogWarning("Lua files not found...");

            //TODO FELIPE MUNIZ -> LOAD LUA FILES

        }
        
        private void RegisterLuaServices()
        {
            Console.WriteLine("Recording lua services...");

            _script = new Script();
            RegisterLuaService("player", typeof(LuaPlayerService));
            ExecuteLuaScript();
        }

        private void ExecuteLuaScript()
        {
            Console.WriteLine("Executing lua script...");
            _script.DoString(@"print('Player name: ' .. player:getNameById(1));");
        }


        public static void RegisterLuaService(string globalName, Type type)
        {
            UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
            _script.Globals[globalName] = _provider.GetService(type);
        }

    }
}
