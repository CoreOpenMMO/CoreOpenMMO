using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SharpServer.Server.Entities;

namespace SharpServer.Server
{
    class Program
    {
        public static ManualResetEvent AllDone = new ManualResetEvent(false);
        public static List<Client> Clients { get; set; }

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

                    Clients = new List<Client>();

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

            var client = new Client(handler.RemoteEndPoint.ToString());

            Clients.Add(client);
            Console.WriteLine($"New connection from client, Guid {client.ClientId} - Adress: {client.Adress}");

            StateObject state = new StateObject { WorkSocket = handler };
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.WorkSocket;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.StringBuilder.Append(Encoding.ASCII.GetString(
                    state.Buffer, 0, bytesRead));

                var content = state.StringBuilder.ToString();

                Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);
                Send(handler, $"Hello client {Clients.Last().ClientId}");
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

        static void Main(string[] args)
        {
            StartListening();
            Console.ReadLine();
        }
    }
}
