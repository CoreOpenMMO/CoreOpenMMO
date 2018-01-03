using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using SharpServer.Server.Entities;
using SharpServer.Server.LuaServices;
using SharpServer.Server.Packets;
using SharpServer.Server.Utils;

namespace SharpServer.Server
{
    class Program
    {
        static ManualResetEvent AllDone = new ManualResetEvent(false);
        static List<Client> Clients { get; set; }
        static NetworkMessage NetworkMessage { get; set; }
        static uint[] xteaKey = new uint[4];

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

            NetworkMessage = new NetworkMessage(6) { WorkSocket = handler };
            handler.BeginReceive(NetworkMessage.Buffer, 0, NetworkMessage.BufferSize, 0, ReadCallback, NetworkMessage);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            try {
             
                NetworkMessage.Length = NetworkMessage.WorkSocket.EndSend(ar);
                
                ByteBuffer buffer = new ByteBuffer();
                buffer.WriteBytes(NetworkMessage.Buffer);
                buffer.Length2 = NetworkMessage.Length;

                var protocol = buffer.ReadByte();
                var os = buffer.ReadInteger();
                var version = buffer.ReadInteger();

                if (version >= 971)
                    buffer.SkipBytes(17);
                else
                    buffer.SkipBytes(12);

                buffer.RSADecrypt();

                //var version = buffer.ReadInteger();

                xteaKey[0] = (uint)buffer.ReadInteger();
                xteaKey[1] = (uint)buffer.ReadInteger();
                xteaKey[2] = (uint)buffer.ReadInteger();
                xteaKey[3] = (uint)buffer.ReadInteger();

                var accountName = buffer.ReadString();
                var password = buffer.ReadString();


                //byte protocol = NetworkMessage.GetByte();
                //var os = NetworkMessage.GetUInt16(); 

                //var version = NetworkMessage.GetUInt16();

                //if (version >= 971)
                //    NetworkMessage.SkipBytes(17);
                //else
                //    NetworkMessage.SkipBytes(12);

                //NetworkMessage.RSADecrypt();

                //xteaKey[0] = NetworkMessage.GetUInt32();
                //xteaKey[1] = NetworkMessage.GetUInt32();
                //xteaKey[2] = NetworkMessage.GetUInt32();
                //xteaKey[3] = NetworkMessage.GetUInt32();

                //var accountName = NetworkMessage.GetString();
                //var password = NetworkMessage.GetString();
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


        private static Script _script;

        static void Main(string[] args)
        {
            _script = new Script();

            RegisterLuaService("player", typeof(LuaPlayerService));
            RegisterLuaService("account", typeof(LuaAccountService));

            ExecuteLuaScript();

            StartListening();
            
            Console.ReadLine();
        }

        public static void RegisterLuaService(string globalName, Type type)
        {
            UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
            _script.Globals[globalName] = Activator.CreateInstance(type);
        }

        //TODO delete this method
        private static void ExecuteLuaScript()
        {
            _script.DoString(@"print('name: ' .. player:getPlayerNameByGuid(1));");
        }
    }
}
