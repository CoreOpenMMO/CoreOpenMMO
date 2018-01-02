using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using SharpServer.Server.Entities;
using SharpServer.Server.LuaServices;
using SharpServer.Server.Packets;

namespace SharpServer.Server
{
    class Program
    {
        public static ManualResetEvent AllDone = new ManualResetEvent(false);
        public static List<Client> Clients { get; set; }
        
        static NetworkMessage inMessage = new NetworkMessage(0);
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

            StateObject state = new StateObject { WorkSocket = handler };
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.WorkSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.StringBuilder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                    inMessage.Buffer = state.Buffer;

                    try
                    {
                        byte protocol = inMessage.GetByte(); // protocol id (1 = login, 2 = game)

                        if (protocol == 1)
                        {

                            var os = inMessage.GetUInt16(); // OS
                            var version = inMessage.GetUInt16(); // version

                            if (version >= 971)
                            {
                                inMessage.SkipBytes(17);
                            }
                            else
                            {
                                inMessage.SkipBytes(12);
                            }

                            inMessage.RSADecrypt();

                            xteaKey[0] = inMessage.GetUInt32();
                            xteaKey[1] = inMessage.GetUInt32();
                            xteaKey[2] = inMessage.GetUInt32();
                            xteaKey[3] = inMessage.GetUInt32();

                            var accountName = inMessage.GetString(); // account name
                            var password = inMessage.GetString(); // password


                            //if (bytesRead > 0)
                            //{
                            //    state.StringBuilder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                            //    var content = state.StringBuilder.ToString();

                            //    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);
                            //    Send(handler, $"Hello client {Clients.Last().ClientId}");
                            //}
                        }
                        else if (protocol == 2)
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                    }



                    // Get the rest of the data.
                    client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    //// All the data has arrived; put it in response.
                    //if (state.StringBuilder.Length > 1)
                    //{
                    //    response = state.sb.ToString();
                    //}
                    //// Signal that all bytes have been received.
                    //receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    
        private static void Receive(Socket client)
        {
            try
            {
            // Create the state object.
            StateObject state = new StateObject();
            state.WorkSocket = client;

            // Begin receiving the data from the remote device.
            client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
            new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
            Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.WorkSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.StringBuilder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                    
                    // Get the rest of the data.
                    client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    //// All the data has arrived; put it in response.
                    //if (state.StringBuilder.Length > 1)
                    //{
                    //    response = state.sb.ToString();
                    //}
                    //// Signal that all bytes have been received.
                    //receiveDone.Set();
                }
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
            RegisterLuaService("account", typeof(LuaPlayerService));

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
