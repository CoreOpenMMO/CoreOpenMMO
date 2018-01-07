using System;
using System.Net;
using System.Net.Sockets;
using COTS.Domain.Interfaces.Services;

namespace COTS.GameServer.Network.Protocols
{
    public class ProtocolGame
    {
        private readonly IPlayerService _playerService;
        private readonly IAccountService _accountService;
        private readonly TcpListener _gameListener;

        public ProtocolGame(IPlayerService playerService, IAccountService accountService)
        {
            _playerService = playerService;
            _accountService = accountService;
            _gameListener = new TcpListener(IPAddress.Any, 7172);
        }

        public void StartListening()
        {
            try
            {
                _gameListener.Start();
                _gameListener.BeginAcceptSocket(GameListenerCallback, _gameListener);
                Console.WriteLine("Game server online!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void GameListenerCallback(IAsyncResult ar)
        {
            try
            {
                var connection = new ConnectionManager(_playerService, _accountService);
                connection.GameListenerCallback(ar);

                _gameListener.BeginAcceptSocket(GameListenerCallback, _gameListener);
                Console.WriteLine("New client connected to game server.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
