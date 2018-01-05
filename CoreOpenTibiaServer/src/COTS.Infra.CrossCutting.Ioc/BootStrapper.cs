using COTS.Data.Context;
using COTS.Data.Repositories;
using COTS.Domain.Interfaces.Repositories;
using COTS.Domain.Interfaces.Services;
using COTS.Domain.Services;
using SimpleInjector;

namespace COTS.Infra.CrossCutting.Ioc
{
    public class BootStrapper
    {
        public static void RegisterServices(Container container)
        {
            
            // Domain
            container.Register<IPlayerService, PlayerService>(Lifestyle.Scoped);
            container.Register<IAccountService, AccountService>(Lifestyle.Scoped);
            
            // Data
            container.Register<IPlayerRepository, PlayerRepository>(Lifestyle.Scoped);
            container.Register<IAccountRepository, AccountRepository>(Lifestyle.Scoped);
            
            container.Register<COTSContext>(Lifestyle.Scoped);
        }
    }
}
