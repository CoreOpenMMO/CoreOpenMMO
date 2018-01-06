using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using COTS.Domain.Interfaces.Services;
using COTS.Infra.CrossCutting.Security;

namespace COTS.GameServer.Network
{
    public class AsynchronousSocketListener
    {
        private static readonly ManualResetEvent AllDone = new ManualResetEvent(false);
        private static NetworkMessage NetworkMessage { get; set; }

        private IPlayerService _playerService;
        private IAccountService _accountService;

        public AsynchronousSocketListener(IAccountService accountService, IPlayerService playerService)
        {
            _accountService = accountService;
            _playerService = playerService;
        }

        public void StartListening()
        {

            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 7171);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                Console.WriteLine("Server online!");

                while (true)
                {
                    AllDone.Reset();

                    listener.BeginAccept(AcceptCallback, listener);

                    AllDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.Read();
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            AllDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            Console.WriteLine($"New connection from client!");
            NetworkMessage = new NetworkMessage(handler);

            handler.BeginReceive(NetworkMessage.Buffer, 0, NetworkMessage.Buffer.Length, 0, ReadCallback, NetworkMessage);

        }

        public async void ReadCallback(IAsyncResult ar)
        {
            try
            {
                NetworkMessage = new NetworkMessage(NetworkMessage.Buffer, NetworkMessage.Handler);

                var protocol = NetworkMessage.GetByte();
                var os = NetworkMessage.GetInt16();
                var version = NetworkMessage.GetInt16();

                if (version >= 971)
                    NetworkMessage.SkipBytes(17);
                else
                    NetworkMessage.SkipBytes(12);

                Rsa.SetKey("14299623962416399520070177382898895550795403345466153217470516082934737582776038882967213386204600674145392845853859217990626450972452084065728686565928113",
                           "7630979195970404721891201847792002125535401292779123937207447574596692788513647179235335529307251350570728407373705564708871762033017096809910315212884101");
                
                var key = new uint[4];

                NetworkMessage.RsaDecrypt();
                NetworkMessage.XteaDecrypt(key);

                NetworkMessage.SkipBytes(14);

                var username = NetworkMessage.GetString();
                var password = NetworkMessage.GetString();
                //var password = SHA1.Create(NetworkMessage.GetString());

                Console.WriteLine($"New player connection: {username}");

                NetworkMessage.SkipBytes((NetworkMessage.Length - 128) - NetworkMessage.Position);

                //var token = NetworkMessage.GetString();

                var account = _accountService.GetAccountByLogin(username, password);

                if (account == null)
                {
                    DisconnectClient("Account name or password is not correct.", version);
                    return;
                }

                account.Characters = _playerService.GetCharactersListByAccountId(account.AccountId);
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

                //Add session key
                output.AddByte(0x28);
                output.AddString(username + "\n" + password + "\n" + token + "\n" + ticks);

                ////Add char list
                output.AddByte(0x64);

                output.AddByte(1); // number of worlds

                output.AddByte(0); // world id
                output.AddString("COTS"); // ServerName
                output.AddString("127.0.0.1"); //IP
                output.AddInt16(7171); // GAME PORT
                output.AddByte(0);

                byte charmax = 0xff;
                
                var size = (byte)Math.Min(charmax, account.Characters.Count);
                output.AddByte(size);

                account.Characters.ForEach(c =>
                {
                    output.AddByte(0);
                    output.AddString(c);
                });

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

                Send(output);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        void DisconnectClient(string message, int version)
        {
            var output = new OutputMessage();
            output.AddByte(version >= 1076 ? (byte)0x0B : (byte)0x0A);
            output.AddString(message);
            Send(output);
        }


        private void Send(OutputMessage output)
        {
            NetworkMessage.Handler.BeginSend(output.Buffer, 0, output.Length, 0, SendCallback, NetworkMessage.Handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;

                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
