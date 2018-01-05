using COTS.Infra.CrossCutting.Network;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace COTS.GameServer.Network {

    public class AsynchronousSocketListener {
        private static readonly ManualResetEvent AllDone = new ManualResetEvent(false);
        private static NetworkMessage NetworkMessage { get; set; }

        public static void StartListening() {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 7171);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true) {
                    AllDone.Reset();

                    Console.WriteLine("Server online!");
                    listener.BeginAccept(AcceptCallback, listener);

                    AllDone.WaitOne();
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            Console.Read();
        }

        public static void AcceptCallback(IAsyncResult ar) {
            AllDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            Console.WriteLine($"New connection from client!");

            // NetworkMessage = new NetworkMessage() { WorkSocket = handler };
            // handler.BeginReceive(NetworkMessage.Buffer, 0, NetworkMessage.Buffer.Length, 0, ReadCallback, NetworkMessage);
        }

        public static void ReadCallback(IAsyncResult ar) {
            try {
                //var protocol = NetworkMessage.ReadByte();
                //var os = NetworkMessage.ReadInteger();
                //var version = NetworkMessage.ReadInteger();

                //if (version >= 971)
                //    NetworkMessage.SkipBytes(17);
                //else
                //    NetworkMessage.SkipBytes(12);

                //NetworkMessage.SkipBytes(13);

                //var accountName = NetworkMessage.ReadString();
                //var password = NetworkMessage.ReadString();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Socket handler, String data) {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, handler);
        }

        private static void SendCallback(IAsyncResult ar) {
            try {
                Socket handler = (Socket)ar.AsyncState;

                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}