using System;
using COTS.Data.Context;
using COTS.Data.Repositories;
using COTS.Domain.Interfaces.Repositories;
using COTS.Domain.Interfaces.Services;
using COTS.Domain.LuaServices;
using COTS.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace COTS.Infra.CrossCutting.Ioc
{
    public class BootStrapper
    {
        public static void ConfigureGlobalServices(IServiceCollection serviceCollection)
        {
            // add configured instance of logging
            serviceCollection.AddSingleton(new LoggerFactory()
                .AddConsole()
                .AddDebug());

            serviceCollection.AddLogging();
            serviceCollection.AddOptions();

            // add services 
            serviceCollection.AddTransient<IPlayerService, PlayerService>();
            serviceCollection.AddTransient<IPlayerRepository, PlayerRepository>();

            //add repository
            serviceCollection.AddTransient<IAccountService, AccountService>();
            serviceCollection.AddTransient<IAccountRepository, AccountRepository>();

            // add app
            serviceCollection.AddTransient<LuaPlayerService>();
            serviceCollection.AddTransient<COTSContext>();

        }
    }
}
