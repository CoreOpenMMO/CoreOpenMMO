using System;
using System.Net;

namespace COTS.GameServer.CommandLineArgumentsParsing {

    public sealed partial class CommandLineArguments {

        public CommandLineArguments(
            string dataDirectoryPath,
            int clientConnectionPort
            ) {
            DataDirectoryPath = string.IsNullOrEmpty(dataDirectoryPath)
                ? "Data"
                : dataDirectoryPath;

            ClientConnectionPort = clientConnectionPort;

            ThrowIfArgumentsAreInvalid();
        }

        private void ThrowIfArgumentsAreInvalid() {
            if (ClientConnectionPort < IPEndPoint.MinPort || ClientConnectionPort > IPEndPoint.MaxPort) {
                var exceptionMessage = nameof(ClientConnectionPort) + $" must be between {IPEndPoint.MinPort} and {IPEndPoint.MaxPort}.";
                throw new ArgumentOutOfRangeException(exceptionMessage);
            }
        }
    }
}