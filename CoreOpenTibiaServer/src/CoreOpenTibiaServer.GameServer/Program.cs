using CommandLine;
using COTS.GameServer.Lua;
using System;
using System.Threading.Tasks;
using COTS.Infra.CrossCutting.Network;

namespace COTS.GameServer {

    public sealed class Program {

        private static void Main(string[] args) {
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
            Task.WaitAny(LuaManager.Initialize());

            var clientConnectionManager = commandLineArguments.GetClientConnectionManager();
            Task.Run(() => clientConnectionManager.StartListening());
        }

        private static void ReportCommandLineParsingError(NotParsed<CommandLineArguments> failedAttempt) {
            throw new NotImplementedException();
        }
    }
}