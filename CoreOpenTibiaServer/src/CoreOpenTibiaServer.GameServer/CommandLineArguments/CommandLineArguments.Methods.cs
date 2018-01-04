using COTS.GameServer.Network;

namespace COTS.GameServer.CommandLineArguments {

    public sealed partial class CommandLineArguments {

        public ConnectionManager GetClientConnectionManager() {
            return new ConnectionManager(port: ClientConnectionPort);
        }
    }
}