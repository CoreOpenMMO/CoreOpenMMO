using CommandLine;
using COTS.GameServer.CommandLineArgumentsParsing;
using COTS.GameServer.Lua;
using COTS.GameServer.World.Loading;
using COTS.Infra.CrossCutting.Ioc;
using COTS.Infra.CrossCutting.Network;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace COTS.GameServer {

    public sealed class Program {
        private static ServiceProvider _serviceProvider;

        private static void Main(string[] args) {
            var worldBytes = File.ReadAllBytes(@"C:\Source\Otxserver-New-master\data\world\realmap.otbm");

            var sw = new Stopwatch();
            sw.Start();
            var world = WorldLoader.ParseWorld(worldBytes);
            sw.Stop();
            Console.WriteLine("Time to parse world: " + sw.ElapsedMilliseconds);

            //var rootHeader = world.GetWorldRootNodeHeader();
            //var versionBytes = BitConverter.GetBytes(rootHeader.WorldEncodingVersion);
            //Array.Reverse(versionBytes);
            //var version = BitConverter.ToUInt32(versionBytes, 0);

            //Console.WriteLine($"World encoding: {rootHeader.WorldEncodingVersion}");
            //Console.WriteLine($"World width {rootHeader.WorldWidth}");
            //Console.WriteLine($"World height {rootHeader.WorldHeight}");
            //Console.WriteLine($"Item encoding major version {rootHeader.ItemEncodingMajorVersion}");
            //Console.WriteLine($"Item encoding minor version {rootHeader.ItemEncodingMinorVersion}");

            Console.WriteLine("Done!!");
            return;
            //var startCount = 0;
            //var stopCount = 0;

            //for (int i = 0; i < worldBytes.Length; i++) {
            //    switch ((WorldNode.NodeMarker)worldBytes[i]) {
            //        case WorldNode.NodeMarker.Escape:
            //        i++;
            //        break;

            //        case WorldNode.NodeMarker.Start:
            //        startCount++;
            //        break;

            //        case WorldNode.NodeMarker.End:
            //        stopCount++;
            //        break;

            //        default:
            //        break;
            //    }
            //}

            //Console.WriteLine("start count: " + startCount);
            //Console.WriteLine("stop count: " + stopCount);            

            var serviceCollection = new ServiceCollection();
            BootStrapper.ConfigureGlobalServices(serviceCollection);
            ConfigureLocalServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            var parser = Parser.Default;
            var parseAttempt = parser.ParseArguments<CommandLineArguments>(args: args);

            if (parseAttempt is Parsed<CommandLineArguments> successfullyParsed) {
                RunWithSucessfullyParsedCommandLineArguments(successfullyParsed.Value);
            } else if (parseAttempt is NotParsed<CommandLineArguments> failedAttempt) {
                ReportCommandLineParsingError(failedAttempt);
            } else {
                throw new InvalidOperationException("Fo reals? This line should never be reached.");
            }

            var original = "testiculos";
            var encoded = NetworkMessage.Encode(original);
            Console.WriteLine(encoded.Length);
            var decoded = NetworkMessage.Decode(encoded);
            Console.WriteLine(original == decoded);

            Console.ReadLine();
        }

        private static void RunWithSucessfullyParsedCommandLineArguments(CommandLineArguments commandLineArguments) {
            _serviceProvider.GetService<LuaManager>().Run();

            var clientConnectionManager = commandLineArguments.GetClientConnectionManager();
            Task.Run(() => clientConnectionManager.StartListening());
        }

        private static void ReportCommandLineParsingError(NotParsed<CommandLineArguments> failedAttempt) {
            throw new NotImplementedException();
        }

        public static void ConfigureLocalServices(IServiceCollection serviceCollection) {
            serviceCollection.AddTransient<LuaManager>();
        }
    }
}