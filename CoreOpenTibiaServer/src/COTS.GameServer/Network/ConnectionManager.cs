using System;
using System.Net;
using System.Net.Sockets;
using COTS.Infra.CrossCutting.Security;

namespace COTS.GameServer.Network {

    public sealed class ConnectionManager {
        private const int _backlogSize = 128;
        private readonly int _port;
        private readonly byte[] _buffer;
        

        public ConnectionManager(int port) {
            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort) {
                var exceptionMessage = nameof(port) + $" must be between {IPEndPoint.MinPort} and {IPEndPoint.MaxPort}.";
                throw new ArgumentOutOfRangeException(exceptionMessage);
            }

            this._buffer = new byte[Constants.NetworkMessageSizeMax];
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

            using (var stream = new NetworkStream(connection))
            {
                stream.BeginRead(_buffer, 0, 2, ClientReadFirstCallBack, null);
            }
            Console.WriteLine($"Server accepted new connection: {connection.RemoteEndPoint.ToString()}");
        }

        private void ClientReadFirstCallBack(IAsyncResult ar)
        {
            var networkMessage = new NetworkMessage(_buffer);

            var protocol = networkMessage.GetByte();
            var os = networkMessage.GetInt16();
            var version = networkMessage.GetInt16();

            if (version >= 971)
                networkMessage.SkipBytes(17);
            else
                networkMessage.SkipBytes(12);

            Rsa.SetKey("14299623962416399520070177382898895550795403345466153217470516082934737582776038882967213386204600674145392845853859217990626450972452084065728686565928113",
                "7630979195970404721891201847792002125535401292779123937207447574596692788513647179235335529307251350570728407373705564708871762033017096809910315212884101");

            var key = new uint[4];

            networkMessage.RsaDecrypt();

            networkMessage.XteaDecrypt(key);

            networkMessage.SkipBytes(14);

            var accountName = networkMessage.GetString();
            var password = networkMessage.GetString();
        }
    }
}