using COTS.GameServer.Network;

namespace COTS.GameServer.CommandLineArgumentsParsing {

    public sealed partial class CommandLineArguments {

        public ConnectionManager GetClientConnectionManager() {
            return new ConnectionManager(port: ClientConnectionPort);
        }
    }
}