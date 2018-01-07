using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace COTS.Infra.CrossCutting.Ioc
{
    using Data.Context;
    using Data.Repositories;
    using Domain.Interfaces.Repositories;
    using Domain.Interfaces.Services;
    using Domain.LuaServices;
    using Domain.Services;
    using System.Linq;
    using System.Reflection;

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

            // All Repositories
            var repositoryAssembly = typeof(AccountRepository).GetTypeInfo().Assembly;
            var repositoryRegistrations =
                from type in repositoryAssembly.GetExportedTypes()
                where type.Namespace == "COTS.Data.Repositories"
                where type.GetInterfaces().Any()
                select new
                {
                    Contract = type.GetInterfaces().First(x => x.Name != typeof(IRepositoryBase<>).Name),
                    Implementation = type
                };

            foreach (var reg in repositoryRegistrations)
            {
                serviceCollection.AddScoped(reg.Contract, reg.Implementation);
            }

            // All Services
            var serviceAssembly = typeof(AccountService).GetTypeInfo().Assembly;
            var serviceRegistrations =
                from type in serviceAssembly.GetExportedTypes()
                where type.Namespace == "COTS.Domain.Services"
                where type.GetInterfaces().Any()
                select new
                {
                    Contract = type.GetInterfaces().First(x => x.Name != typeof(IServiceBase<>).Name),
                    Implementation = type
                };

            foreach (var reg in serviceRegistrations)
            {
                serviceCollection.AddScoped(reg.Contract, reg.Implementation);
            }

            // add app
            serviceCollection.AddTransient<LuaPlayerService>();
            serviceCollection.AddTransient<COTSContext>();
        }
    }
}
