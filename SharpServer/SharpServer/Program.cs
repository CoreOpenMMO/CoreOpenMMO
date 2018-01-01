using CommandLine;
using Newtonsoft.Json;
using System;
using System.Linq;

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
            var knight = Vocation.DefaultKnight();
            var vocations = ReadOnlyArray<Vocation>.WrapCollection(new Vocation[] { knight });
            var serialized = JsonConvert.SerializeObject(vocations, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<ReadOnlyArray<Vocation>>(serialized, settings: new JsonSerializerSettings() { Formatting = Formatting.Indented });
            Console.WriteLine(serialized);
            Console.WriteLine(Enumerable.SequenceEqual(vocations, deserialized));
        }

        private static void ReportCommandLineParsingError(NotParsed<CommandLineArguments> failedAttempt) {
            throw new NotImplementedException();
        }
    }
}