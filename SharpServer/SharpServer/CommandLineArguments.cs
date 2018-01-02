using CommandLine;

namespace SharpServer {

    public sealed class CommandLineArguments {

        [Option(
            longName: nameof(DataDirectoryPath),
            Required = false,
            Default = "Data",
            HelpText = "Relative or absolute path to server's Data directory."
            )]
        public string DataDirectoryPath { get; }

        public CommandLineArguments(string dataDirectoryPath) {
            if (string.IsNullOrEmpty(dataDirectoryPath))
                DataDirectoryPath = "Data";
            else
                DataDirectoryPath = dataDirectoryPath;
        }
    }
}