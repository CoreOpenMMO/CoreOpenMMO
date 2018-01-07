using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using COTS.Domain.Interfaces.Services;
using COTS.Infra.CrossCutting.Security;
using COTS.Infra.CrossCutting.Security.Enums;

namespace COTS.GameServer.Network
{
    public sealed class ConnectionManager
    {
        private Socket _handler;
        private NetworkStream _networkStream;
        private NetworkMessage _networkMessage;

        private readonly IPlayerService _playerService;
        private readonly IAccountService _accountService;

        public ConnectionManager(IPlayerService playerService, IAccountService accountService)
        {
            _playerService = playerService;
            _accountService = accountService;
        }

        public void LoginListenerCallback(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("LoginListenerCallback");

                TcpListener clientListener = (TcpListener)ar.AsyncState;
                _handler = clientListener.EndAcceptSocket(ar);
                _networkStream = new NetworkStream(_handler);
                _networkMessage = new NetworkMessage();
                _networkStream.BeginRead(_networkMessage.Buffer, 0, 2, ReceiveLoginMessageCallBack, _handler);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CloseConnection();
            }
        }

        public void GameListenerCallback(IAsyncResult ar)
        {
            try {

                Console.WriteLine("GameListenerCallback");

                TcpListener gameListener = (TcpListener)ar.AsyncState;
                _handler = gameListener.EndAcceptSocket(ar);
                _networkStream = new NetworkStream(_handler);
                _networkMessage = new NetworkMessage();
                SendPacketConnection();

                _networkStream.BeginRead(_networkMessage.Buffer, 0, 2, ReceiveLoginMessageCallBack, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CloseConnection();
            }
        }

        private void ReceiveLoginMessageCallBack(IAsyncResult ar)
        {
            try
            {
                if (!EndReadAyncResult(ar)) return;

                Console.WriteLine("ReceiveFirstMessageCallBack");
                
                var protocol = (ClientPacketType)_networkMessage.GetByte();

                switch (protocol)
                {
                    case ClientPacketType.LoginServerRequest:
                        ReceiveLoginFirstMessage();
                        break;
                    case ClientPacketType.GameServerRequest:
                        ReceiveMessage();
                        break;
                    default:
                        Console.WriteLine("Protocol not found.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CloseConnection();
            }
        }

        private void ReceiveMessage()
        {
            try { 
                Console.WriteLine("ReceiveLoginMessage");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CloseConnection();
            }
        }

        private bool EndReadAyncResult(IAsyncResult ar)
        {
            try
            {
                int read = _networkStream.EndRead(ar);

                if (read == 0)
                {
                    CloseConnection();
                    return false;
                }

                int size = (int)BitConverter.ToUInt16(_networkMessage.Buffer, 0) + 2;

                while (read < size)
                {
                    if (_networkStream.CanRead)
                        read += _networkStream.Read(_networkMessage.Buffer, read, size - read);
                }
                _networkMessage.Length = size;

                _networkMessage.Position = 0;

                _networkMessage.GetUInt16();
                _networkMessage.GetUInt32(); 

                return true;
            }
            catch
            {
                CloseConnection();
                return false;
            }
        }

        private void ReceiveLoginFirstMessage()
        {
            try
            {
                var os = _networkMessage.GetInt16();
                var version = _networkMessage.GetInt16();

                if (version >= 971)
                    _networkMessage.SkipBytes(17);
                else
                    _networkMessage.SkipBytes(12);

                Rsa.SetKey("14299623962416399520070177382898895550795403345466153217470516082934737582776038882967213386204600674145392845853859217990626450972452084065728686565928113",
                           "7630979195970404721891201847792002125535401292779123937207447574596692788513647179235335529307251350570728407373705564708871762033017096809910315212884101");

                _networkMessage.RsaDecrypt();

                _networkMessage.Key[0] = _networkMessage.GetUInt32();
                _networkMessage.Key[1] = _networkMessage.GetUInt32();
                _networkMessage.Key[2] = _networkMessage.GetUInt32();
                _networkMessage.Key[3] = _networkMessage.GetUInt32();

                var username = _networkMessage.GetString();
                var password = _networkMessage.GetString();

                _networkMessage.SkipBytes((_networkMessage.Length - 128) - _networkMessage.Position);


                var account = _accountService.GetAccountByLogin(username, password);

                if (account == null)
                {
                    DisconnectClient("Account name or password is not correct.", version);
                    return;
                }

                account.Characters = _playerService.GetCharactersListByAccountId(account.AccountId).ToList();

                Console.WriteLine($"\nNew login from: {_handler.RemoteEndPoint} \n" +
                                    $"Account: {account.UserName} \n" +
                                    $"Characters: {account.Characters.Count} \n" +
                                    $"PremiumDays: {account.PremiumDays} \n");

                var output = new OutputMessage();

                long ticks = DateTime.Now.Ticks / 30;

                var token = "";

                if (!string.IsNullOrEmpty(account.Password))
                {
                    //    if (token.empty() || !(token == generateToken(account.key, ticks) || token == generateToken(account.key, ticks - 1) || token == generateToken(account.key, ticks + 1)))
                    //    {
                    //        output->addByte(0x0D);
                    //        output->addByte(0);
                    //        send(output);
                    //        disconnect();
                    //        return;
                    //    }
                    output.AddByte(0x0C);
                    output.AddByte(0);
                }

                ////Update premium days
                //Game::updatePremium(account);

                output.AddByte(0x14);

                var motd = "0\nBem vindo ao COTS!";

                output.AddString(motd);

                if (version > 1071)
                { //Add session key
                    output.AddByte(0x28);
                    output.AddString(username + "\n" + password + "\n" + token + "\n" + ticks);
                }

                //Add char list
                output.AddByte(0x64);

                byte charmax = 0xff;
                var size = (byte)Math.Min(charmax, account.Characters.Count);
                var serverName = "COTS";
                var serverIp = "127.0.0.1";
                Int16 gamePort = 7172;
                if (version > 1010)
                {
                    output.AddByte(1); // number of worlds
                    output.AddByte(0); // world id
                    output.AddString(serverName);
                    output.AddString(serverIp);
                    output.AddInt16(gamePort);
                    output.AddByte(0);
                    output.AddByte(size);

                    account.Characters.ForEach(c =>
                    {
                        output.AddByte(0);
                        output.AddString(c);
                    });
                }
                else
                {
                    var ipAddress = IPAddress.Parse(serverIp);
                    var ipBytes = ipAddress.GetAddressBytes();
                    var serverIPAdress = (uint)ipBytes[0] << 24;
                    serverIPAdress += (uint)ipBytes[1] << 16;
                    serverIPAdress += (uint)ipBytes[2] << 8;
                    serverIPAdress += (uint)ipBytes[3];

                    output.AddByte((byte)(account.Characters.Count));
                    account.Characters.ForEach(c =>
                    {
                        output.AddString(c);
                        output.AddString(serverName);
                        output.AddUInt32(serverIPAdress);
                        output.AddInt16(gamePort);
                    }
                    );
                }

                var frepremium = true;

                //Add premium days
                output.AddByte(0);
                if (frepremium)
                {
                    output.AddByte(1);
                    output.AddInt32(0);
                }
                else
                {
                    output.AddByte(account.PremiumDays > 0 ? (byte)1 : (byte)0);
                    output.AddInt32((int)(DateTime.Now.Ticks + (account.PremiumDays * 86400)));
                }

                SendMessage(output);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CloseConnection();
            }
        }
        
        private void CloseConnection()
        {
            try { 
                _networkStream.Close();
                _handler.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SendPacketConnection()
        {
            try
            {
                OutputMessage message = new OutputMessage();

                message.AddByte(0x1F); // type

                message.AddUInt32(0x1337); // time in seconds since server start

                message.AddByte(0x10); // fractional time?

                SendMessage(message, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CloseConnection();
            }
        }

        void DisconnectClient(string message, int version)
        {
            try
            {
                var output = new OutputMessage();
                output.AddByte(version >= 1076 ? (byte)0x0B : (byte)0x0A);
                output.AddString(message);

                SendMessage(output);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CloseConnection();
            }
        }

        private void SendMessage(OutputMessage output, bool useEncrypt = true)
        {
            try
            {
                output.WriteMessageLength();
                if (useEncrypt)
                {
                    XTea.EncryptXtea(ref output, _networkMessage.Key);
                    output.AddCryptoHeader(true);
                }
                else
                {
                    output.InsertAdler32();
                    output.InsertTotalLength();
                }

                //_handler.BeginSend(output.Buffer, 0, output.Length, 0, SendMessageCallback, _handler);
                _networkStream.BeginWrite(output.Buffer, 0, output.Length, null, null);

            }
            catch (ObjectDisposedException)
            {
                CloseConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CloseConnection();
            }
        }

        private void SendMessageCallback(IAsyncResult ar)
        {
            try
            {
                CloseConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
