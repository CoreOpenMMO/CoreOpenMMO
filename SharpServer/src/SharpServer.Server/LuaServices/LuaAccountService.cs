using MoonSharp.Interpreter;
using SharpServer.Database;

namespace SharpServer.Server.LuaServices
{
    [MoonSharpUserData]
    public class LuaAccountService
    {
        private static AccountRepository _accountRepository;

        public LuaAccountService()
        {
            _accountRepository = new AccountRepository();
        }
    }
}
