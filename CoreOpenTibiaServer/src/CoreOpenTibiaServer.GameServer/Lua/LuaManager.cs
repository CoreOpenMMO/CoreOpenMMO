using System;
using System.Threading.Tasks;
using COTS.Domain.LuaServices;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace COTS.GameServer.Lua
{
    public class LuaManager
    {

        public static void RegisterLuaService(string globalName, Type type)
        {
            UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
            _script.Globals[globalName] = Activator.CreateInstance(type);
        }
        
        private static Script _script;

        public static async Task Initialize()
        {
            _script = new Script();

            await LoadLuaFiles();
            await RegisterLuaServices();
        }

        private static async Task LoadLuaFiles()
        {
            Console.WriteLine("Loading lua files...");

            //TODO FELIPE MUNIZ -> LOAD LUA FILES

        }

        private static async Task RegisterLuaServices()
        {
            Console.WriteLine("Registering lua services...");

            _script = new Script();
            RegisterLuaService("player", typeof(LuaPlayerService));
            ExecuteLuaScript();
        }
        
        private static void ExecuteLuaScript()
        {
            _script.DoString(@"print('name: ' .. player:getPlayerNameByGuid(1));");
        }

    }
}
