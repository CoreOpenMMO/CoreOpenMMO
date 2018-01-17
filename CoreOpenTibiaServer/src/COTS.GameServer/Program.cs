using CommandLine;
using COTS.GameServer.CommandLineArgumentsParsing;
using COTS.GameServer.World.Loading;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace COTS.GameServer {

    public sealed class Program {

        private static void Main(string[] args) {
            // var worldBytes = File.ReadAllBytes(@"C:\Source\Otxserver-New-master\data\world\realmap.otbm");
            var worldBytes = File.ReadAllBytes(@"C:\Source\forgottenserver-master\data\world\forgotten.otbm");

            var sw = new Stopwatch();
            sw.Start();
            var parsingTree = WorldLoader.ParseWorld(worldBytes);
            sw.Stop();
            Console.WriteLine("Time to parse world: " + sw.ElapsedMilliseconds + " ms");

            var root = parsingTree.Root;

            var worldHeader = WorldLoader.GetWorldHeader(parsingTree);
            Console.WriteLine($"World encoding: {worldHeader.WorldEncodingVersion}");
            Console.WriteLine($"World width {worldHeader.WorldWidth}");
            Console.WriteLine($"World height {worldHeader.WorldHeight}");
            Console.WriteLine($"Item encoding major version {worldHeader.ItemEncodingMajorVersion}");
            Console.WriteLine($"Item encoding minor version {worldHeader.ItemEncodingMinorVersion}");

            var worldAttributes = WorldLoader.GetWorldAttributes(parsingTree);
            Console.WriteLine($"World description: {worldAttributes.WorldDescription}");
            Console.WriteLine($"Houses filename: {worldAttributes.HousesFilename}");
            Console.WriteLine($"Spawns filename: {worldAttributes.SpawnsFilename}");

            Console.WriteLine("Done!!");
            return;
        }

        private static void RunWithSucessfullyParsedCommandLineArguments(CommandLineArguments commandLineArguments) {
            var clientConnectionManager = commandLineArguments.GetClientConnectionManager();
            Task.Run(() => clientConnectionManager.StartListening());
        }

        private static void ReportCommandLineParsingError(NotParsed<CommandLineArguments> failedAttempt) {
            throw new NotImplementedException();
        }
    }
}