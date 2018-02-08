using COTS.Domain.Interfaces.Services;
using COTS.Infra.CrossCutting.Network;
using COTS.Infra.CrossCutting.Network.Enums;
using COTS.Infra.CrossCutting.Network.Security;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace COTS.GameServer.Network.Protocols {

    public class LoginProtocol {

        #region Properties
        const int loginHeaderSize = 2; //sizeof(int); //Always 4 bytes

        private struct Connection {
            public TcpClient client { get; set; }
            public NetworkStream stream { get; set; }
            public NetworkMessage message { get; set; }
        }

        private readonly IPlayerService _playerService;
        private readonly IAccountService _accountService;
        private readonly TcpListener _loginListener;
        private readonly ConnectionManager _manager;
        private Connection _currentConnection;
        #endregion

        #region Constructor
        public LoginProtocol(IPlayerService playerService, IAccountService accountService) {
            _playerService = playerService;
            _accountService = accountService;
            _loginListener = new TcpListener(localaddr: IPAddress.Any, port: 7171);
            _manager = new ConnectionManager(_playerService, _accountService);

            Console.WriteLine("Login server online!");
        }

        public void Listen() {
            Console.WriteLine("Listening TcpClient for Login!");
            _loginListener.Start();
            while (true) {
                TcpClient tcpclient = _loginListener.AcceptTcpClient();
                Task.Run(() => HandleNewConnection(tcpclient));
            }
        }
        #endregion

        private void HandleNewConnection(TcpClient tcpclient) {
            Console.WriteLine($"Server accepted new connection: {tcpclient.Client.RemoteEndPoint.ToString()}");
            try {
                // Handle Security mesures, DDoS may happen here, since we only Enqueue all connections
                // (maybe limit to 30 login requests per IP).
                _manager.AddConnection(new PendingRequest(tcpclient));
            } catch (Exception e) {
                tcpclient.Client.Close();

                var retinfo = new StringBuilder();
                retinfo.Append("The unhandled connection thread/task id: ");
                retinfo.Append(Thread.CurrentThread.ManagedThreadId);
                retinfo.Append(" encountered an error with the connection endpoint: ");
                retinfo.Append(tcpclient.Client.RemoteEndPoint.ToString());
                retinfo.Append(" - Error: ");
                retinfo.Append(e.Message);

                Console.WriteLine(retinfo);
            }
        }

        public void HandlePendingRequest() {
            PendingRequest retunerUhConnection = null;
            NetworkStream stream = null;
            //try {
                if (_manager.PullConnection(out retunerUhConnection)) {
                    stream = new NetworkStream(retunerUhConnection.Socket.Client);
                    var messageSize = NetworkMessage.ReadMessageSize(stream, loginHeaderSize);
                    var message = new NetworkMessage(stream, messageSize, loginHeaderSize);

                    _currentConnection.client = retunerUhConnection.Socket;
                    _currentConnection.stream = stream;
                    _currentConnection.message = message;

                    ManageLogin();
                } //Else, nothing to be pulled, just go and try to pull another Unhandled Request*/
            /*} catch (Exception e) {
                if(stream != null) {
                    stream.Close();
                }

                var retinfo = new StringBuilder();
                retinfo.Append("The task id: ");
                retinfo.Append(Thread.CurrentThread.ManagedThreadId);
                retinfo.Append(" encountered an error while hadling a pending request");

                if(retunerUhConnection != null){
                    retinfo.Append(". The connection endpoint ip is: ");
                    retinfo.Append(retunerUhConnection.Socket.Client.RemoteEndPoint.ToString());
                    retunerUhConnection.Socket.Client.Close();
                }

                retinfo.Append(". - Error: ");
                retinfo.Append(e.Message);

                Console.WriteLine(retinfo);
            }*/
        }

        public void ManageLogin() {
            NetworkMessage message = _currentConnection.message;

            message.SkipBytes(4);

            if ((ClientPacketType)message.GetByte() != ClientPacketType.LoginServerRequest) {
                throw new Exception("Pending request was not of type LoginServerRequest!");
            }

            var os = message.GetInt16();
            var version = message.GetInt16();

            if (version >= 971)
                message.SkipBytes(17);
            else
                message.SkipBytes(12);

            Rsa.SetKey("14299623962416399520070177382898895550795403345466153217470516082934737582776038882967213386204600674145392845853859217990626450972452084065728686565928113",
                       "7630979195970404721891201847792002125535401292779123937207447574596692788513647179235335529307251350570728407373705564708871762033017096809910315212884101");

            message.RsaDecrypt();

            message.Key[0] = message.GetUInt32();
            message.Key[1] = message.GetUInt32();
            message.Key[2] = message.GetUInt32();
            message.Key[3] = message.GetUInt32();

			/* Fixed Keys for Test 
			key[0] = 3442030272;
			key[1] = 2364789040;
			key[2] = 1503299581;
			key[3] = 3670909886;*/

			var username = message.GetString();
            var password = message.GetString();

            message.SkipBytes((message.Length - 128) - message.Position); //Jumping Offset


            var account = _accountService.GetAccountByLogin(username, password);

            if (account == null) {
                //DisconnectClient("Account name or password is not correct.", version);
                Console.WriteLine("Account name or password is not correct.");
                return;
            }

            account.Characters = _playerService.GetCharactersListByAccountId(account.AccountId).ToList();

            Console.WriteLine($"\nNew login from: {_currentConnection.client.Client.RemoteEndPoint} \n" +
                                $"Account: {account.UserName} \n" +
                                $"Characters: {account.Characters.Count} \n" +
                                $"PremiumDays: {account.PremiumDays} \n");

            var output = new OutputMessage();
			var ticks = DateTime.Now.Ticks / 30;
			/* Fixed TimeStamp for Test
			var ticks = 21217867608090199;*/

			var token = "";

            if (!string.IsNullOrEmpty(account.Password)) {
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

            if (version > 1071) { //Add session key
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
            if (version > 1010) {
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
            else {
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
            if (frepremium) {
                output.AddByte(1);
                output.AddInt32(0);
            }
            else {
                output.AddByte(account.PremiumDays > 0 ? (byte)1 : (byte)0);
                output.AddInt32((int)(DateTime.Now.Ticks + (account.PremiumDays * 86400)));
            }

            //SendMessage(output);
            output.WriteMessageLength();

			/* Adds Padding */
			var pad = output.Length % 8 == 0 ? 0 : 8 - (output.Length % 8);
			output.AddPaddingBytes(pad);

			/* Removes the Head */
			var headerless = new byte[output.Length];
			Array.Copy(output.Buffer, 6, headerless, 0, output.Length);

			/* Encrypts the Message and Copies it back to Output.Buffer */
			var encryptedMessage = XTea.EncryptXtea(headerless, message.Key);
			Array.Copy(encryptedMessage, 0, output.Buffer, 6, encryptedMessage.Length);

			output.AddCryptoHeader(true);
			_currentConnection.stream.Write(output.Buffer, 0, output.Length);
		}
    }
}