using CommandLine;
using System;
using System.IO;

namespace SharpServer {

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