using System;
using System.Net;
using System.Net.Sockets;

namespace COTS.GameServer.Network {

    public sealed class ConnectionManager {
        private const int _backlogSize = 128;
        private readonly int _port;

        public ConnectionManager(int port) {
            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort) {
                var exceptionMessage = nameof(port) + $" must be between {IPEndPoint.MinPort} and {IPEndPoint.MaxPort}.";
                throw new ArgumentOutOfRangeException(exceptionMessage);
            }

            this._port = port;
        }

        /// <remarks>This method doesn't return.</remarks>
        public void StartListening() {
            var listener = new TcpListener(localaddr: IPAddress.Any, port: _port);

            listener.Start();
            Console.WriteLine($"Server started listening on: {_port}");

            while (true) {
                var connection = listener.AcceptSocket();

                HandleNewConnection(connection);
            }
        }

        private void HandleNewConnection(Socket connection) {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            Console.WriteLine($"Server accepted new connection: {connection.RemoteEndPoint.ToString()}");
        }
    }
}