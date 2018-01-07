using System;
using System.Net;
using System.Net.Sockets;
using COTS.Domain.Interfaces.Services;

namespace COTS.GameServer.Network
{
    public class ProtocolGame
    {
        private readonly TcpListener _gameListener;
        private readonly IPlayerService _playerService;
        private readonly IAccountService _accountService;

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
                Console.WriteLine("Game server online!");

                _gameListener.Start();
                _gameListener.BeginAcceptSocket(GameListenerCallback, _gameListener);
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
