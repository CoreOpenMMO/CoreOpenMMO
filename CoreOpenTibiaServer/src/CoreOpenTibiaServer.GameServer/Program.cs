using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using COTS.GameServer.Lua;
using COTS.GameServer.Network;

namespace COTS.GameServer {

    public sealed class Program {

        private static async Task Main(string[] args) {
            var parser = Parser.Default;
            var parseAttempt = parser.ParseArguments<CommandLineArguments>(args: args);

            if (parseAttempt is Parsed<CommandLineArguments> successfullyParsed) {
                RunWithSucessfullyParsedCommandLineArguments(successfullyParsed.Value);
            } else if (parseAttempt is NotParsed<CommandLineArguments> failedAttempt) {
                ReportCommandLineParsingError(failedAttempt);
            } else {
                throw new InvalidOperationException("Fo reals? This line should never be reached.");
            }

            await LuaManager.Initialize();
            AsynchronousSocketListener.StartListening();

            Console.ReadLine();
        }

        private static void RunWithSucessfullyParsedCommandLineArguments(CommandLineArguments value) {
            var globalFilePath = Path.Combine(value.DataDirectoryPath, "global.lua");
            Console.WriteLine(Path.GetFullPath(globalFilePath));
            Console.WriteLine(File.Exists(globalFilePath));
        }

        private static void ReportCommandLineParsingError(NotParsed<CommandLineArguments> failedAttempt) {
            throw new NotImplementedException();
        }
    }
}