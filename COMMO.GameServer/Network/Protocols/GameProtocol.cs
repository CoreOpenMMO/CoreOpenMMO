using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using COMMO.Domain.Interfaces.Services;

namespace COMMO.GameServer.Network.Protocols {
    public class GameProtocol {
        private readonly IPlayerService _playerService;
        private readonly IAccountService _accountService;
        private readonly TcpListener _gameListener;

        public GameProtocol(IPlayerService playerService, IAccountService accountService) {
            _playerService = playerService;
            _accountService = accountService;
            _gameListener = new TcpListener(localaddr: IPAddress.Any, port: 7172);

            Console.WriteLine("Game server online!");
        }

        public void Listen() {
            /*while (true) {
                TcpClient connection = _gameListener.AcceptTcpClient();
                Task.Run(() => HandleNewConnection(connection));
            }*/
        }

        private void HandleNewConnection(TcpClient connection) {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            Console.WriteLine($"Server accepted new connection: {connection.Client.RemoteEndPoint.ToString()}");
            new Thread(() => {

            }).Start();
        }


        public void HandlePendingRequest() {

        }
        /*private void GameListenerCallback(IAsyncResult ar)
        {
            var connection = new ConnectionManager(_playerService, _accountService);
            connection.GameListenerCallback(ar);

            _gameListener.BeginAcceptSocket(GameListenerCallback, _gameListener);
            Console.WriteLine("New client connected to game server.");
        }*/
    }
}
