using COTS.Domain.Interfaces.Services;
using System;
using System.Net;
using System.Net.Sockets;

namespace COTS.GameServer.Network.Protocols
{
    public class ProtocolLogin
    {
        private readonly TcpListener _loginListener;
        private readonly IPlayerService _playerService;
        private readonly IAccountService _accountService;

        public ProtocolLogin(IPlayerService playerService, IAccountService accountService)
        {
            _playerService = playerService;
            _accountService = accountService;
            _loginListener = new TcpListener(IPAddress.Any, 7171);
        }

        public void StartListening()
        {
            try
            {
                Console.WriteLine("Login server online!");

                _loginListener.Start();
                _loginListener.BeginAcceptSocket(LoginListenerCallback, _loginListener);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void LoginListenerCallback(IAsyncResult ar)
        {
            try
            {
                var connection = new ConnectionManager(_playerService, _accountService);
                connection.LoginListenerCallback(ar);

                _loginListener.BeginAcceptSocket(LoginListenerCallback, _loginListener);
                Console.WriteLine("New client connected to login server.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
    }
}
