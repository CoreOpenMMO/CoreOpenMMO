using CommandLine;
using COTS.GameServer.CommandLineArgumentsParsing;
using COTS.GameServer.Lua;
using System;
using COTS.GameServer.Network;
using COTS.GameServer.Network.Protocols;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace COTS.GameServer {

    public sealed class Program {

        private static ServiceProvider _serviceProvider;

        private static void Main(string[] args) {

            var serviceCollection = new ServiceCollection();
            ConfigureLocalServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            var parser = Parser.Default;
            var parseAttempt = parser.ParseArguments<CommandLineArguments>(args: args);

            if (parseAttempt is Parsed<CommandLineArguments> successfullyParsed) {
                RunWithSucessfullyParsedCommandLineArguments(successfullyParsed.Value);
            }
            else if (parseAttempt is NotParsed<CommandLineArguments> failedAttempt) {
                ReportCommandLineParsingError(failedAttempt);
            }
            else {
                throw new InvalidOperationException("Fo reals? This line should never be reached.");
            }

            Console.ReadLine();
        }

        private static void RunWithSucessfullyParsedCommandLineArguments(CommandLineArguments commandLineArguments) {
            _serviceProvider.GetService<LuaManager>().Run();
            LoginProtocol loginServer = _serviceProvider.GetService<LoginProtocol>();
            GameProtocol gameServer = _serviceProvider.GetService<GameProtocol>();

            //Server Main Loop
            Task.Run(() => loginServer.Listen());
            Task.Run(() => gameServer.Listen());

            while (true) { 
                loginServer.HandlePendingRequest();
                gameServer.HandlePendingRequest();
            }

            //var clientConnectionManager = commandLineArguments.GetClientConnectionManager();
            //Task.Run(() => clientConnectionManager.StartListening());
        }

        private static void ReportCommandLineParsingError(NotParsed<CommandLineArguments> failedAttempt) => throw new NotImplementedException();

        public static void ConfigureLocalServices(IServiceCollection serviceCollection) {
            serviceCollection.AddTransient<LuaManager>();
            serviceCollection.AddTransient<ConnectionManager>();
            serviceCollection.AddTransient<LoginProtocol>();
            serviceCollection.AddTransient<GameProtocol>();
        }
    }
}
