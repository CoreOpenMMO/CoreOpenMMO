using CommandLine;
using COTS.GameServer.Network;

namespace COTS.GameServer {

    public sealed class CommandLineArguments {

        [Option(
            longName: nameof(DataDirectoryPath),
            Required = false,
            Default = "Data",
            HelpText = "Relative or absolute path to server's Data directory."
            )]
        public string DataDirectoryPath { get; }

        [Option(
            longName: nameof(ClientConnectionPort),
            Required = false,
            Default = 7171,
            HelpText = "The port clients will connect to."
            )]
        public int ClientConnectionPort { get; }

        public CommandLineArguments(
            string dataDirectoryPath,
            int clientConnectionPort
            ) {
            DataDirectoryPath = string.IsNullOrEmpty(dataDirectoryPath)
                ? "Data"
                : dataDirectoryPath;

            ClientConnectionPort = clientConnectionPort;
        }

        public ConnectionManager GetClientConnectionManager() {
            return new ConnectionManager(port: ClientConnectionPort);
        }
    }
}