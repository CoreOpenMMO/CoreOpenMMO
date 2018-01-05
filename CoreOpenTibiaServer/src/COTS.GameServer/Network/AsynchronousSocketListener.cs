using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using COTS.Infra.CrossCutting.Security;

namespace COTS.GameServer.Network
{
    public class AsynchronousSocketListener
    {
        private static readonly ManualResetEvent AllDone = new ManualResetEvent(false);
        private static NetworkMessage NetworkMessage { get; set; }

        public static void StartListening()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 7171);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    AllDone.Reset();

                    Console.WriteLine("Server online!");
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

        public static void AcceptCallback(IAsyncResult ar)
        {
            AllDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            Console.WriteLine($"New connection from client!");
            NetworkMessage = new NetworkMessage();

            handler.BeginReceive(NetworkMessage.Buffer, 0, NetworkMessage.Buffer.Length, 0, ReadCallback, NetworkMessage);

        }

        public static void ReadCallback(IAsyncResult ar)
        {
            try
            {
                NetworkMessage = new NetworkMessage(NetworkMessage.Buffer);
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

                var accountName = NetworkMessage.GetString();
                var password = NetworkMessage.GetString();

                Console.WriteLine($"New player connection: {accountName}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        private static void Send(Socket handler, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, handler);
        }

        private static void SendCallback(IAsyncResult ar)
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
