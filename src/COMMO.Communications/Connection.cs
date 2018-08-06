// <copyright file="Connection.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Sockets;
using COMMO.Security;
using COMMO.Server.Data;

namespace COMMO.Communications
{
    public class Connection
    {
        private readonly object _writeLock;
		private OpenTibiaProtocolType _openTibiaProtocolType;
		private bool _firstConnection = false;

        public delegate void OnConnectionClose(Connection c);

        public delegate void OnProcess(Connection c, NetworkMessage m);

        public delegate void OnPostProcess(Connection c);

        public event OnConnectionClose OnCloseEvent;

        public event OnProcess OnProcessEvent;

        public event OnPostProcess OnPostProcessEvent;

        public Socket Socket { get; private set; }

        public NetworkStream Stream { get; private set; }

        public NetworkMessage InMessage { get; }

        public uint PlayerId { get; set; }

        public uint[] XTeaKey { get; private set; }

        public bool IsAuthenticated { get; set; }

        public string SourceIp => Socket?.RemoteEndPoint.ToString();

        public Connection(OpenTibiaProtocolType openTibiaProtocolType)
        {
			_openTibiaProtocolType = openTibiaProtocolType;
            _writeLock = new object();
            Socket = null;
            Stream = null;
            InMessage = new NetworkMessage(0);
            XTeaKey = new uint[4];
            IsAuthenticated = false;
        }

        public void BeginStreamRead()
        {
            Stream.BeginRead(InMessage.Buffer, 0, 2, OnRead, null);
        }

        public void OnAccept(IAsyncResult ar)
        {
            if (ar == null)
            {
                throw new ArgumentNullException(nameof(ar));
            }

            Socket = ((TcpListener)ar.AsyncState).EndAcceptSocket(ar);
            Stream = new NetworkStream(Socket);

			//if (!_firstConnection && _openTibiaProtocolType == OpenTibiaProtocolType.GameProtocol) //FirstGameConnection
			//{
			//	_firstConnection = true;

			//	var message = new NetworkMessage(true);

			//	message.AddUInt16(0x0006);
			//	message.AddByte(0x1F);
				
			//	var challengeTimestamp = (uint)Environment.TickCount;

			//	message.AddUInt32(challengeTimestamp); // challengeTimestamp

			//	var challengeRandom = new Random().Next(0x00, 0xFF);

			//	message.AddByte((byte)challengeRandom); // challengeRandom

			//	message.SkipBytes(-6); // go back to header
				
			//	Send(message, false);
			//}

            if (SimpleDoSDefender.Instance.IsBlockedAddress(SourceIp))
            {
                // TODO: evaluate if it is worth just leaving the connection open but ignore it, so that they think they are successfully DoSing...
                // But we would need to think if it is a connection drain attack then...
                Close();
            }
            else
            {
                SimpleDoSDefender.Instance.LogConnectionAttempt(SourceIp);
                BeginStreamRead();
            }
        }

        private void OnRead(IAsyncResult ar)
        {
            if (!CompleteRead(ar))
            {
                return;
            }

            try
            {
                OnProcessEvent?.Invoke(this, InMessage);
                OnPostProcessEvent?.Invoke(this);
            }
            catch (Exception e)
            {
                // Invalid data from the client
                // TODO: I must not swallow exceptions.
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);

                // TODO: is closing the connection really necesary?
                // Close();
            }
        }

        private bool CompleteRead(IAsyncResult ar)
        {
            try
            {
                int read = Stream.EndRead(ar);

                if (read == 0)
                {
                    // client disconnected
                    Close();
                    return false;
                }

                int size = BitConverter.ToUInt16(InMessage.Buffer, 0) + 2;

                while (read < size)
                {
                    if (Stream.CanRead)
                    {
                        read += Stream.Read(InMessage.Buffer, read, size - read);
                    }
                }

                InMessage.Resize(size);
                InMessage.GetUInt16(); // total length

                return true;
            }
            catch (Exception e)
            {
                // TODO: I must not swallow exceptions.
                // TODO: is closing the connection really necesary?
                Console.WriteLine(e.ToString());
                Close();
            }

            return false;
        }

        // private bool isInTransaction = false;
        // private NetworkMessage transactionMessage = new NetworkMessage();

        // public void BeginTransaction()
        // {
        //    if (!isInTransaction)
        //    {
        //        transactionMessage.Reset();
        //        isInTransaction = true;
        //    }
        // }

        // public void CommitTransaction()
        // {
        //    SendMessage(transactionMessage, true);
        //    isInTransaction = false;
        // }
        private void SendMessage(NetworkMessage message, bool useEncryption, bool managementProtocol = false)
        {
            if (useEncryption)
            {
                message.PrepareToSend(XTeaKey);
            }
            else
            {
                message.PrepareToSendWithoutEncryption(managementProtocol);
            }

            try
            {
                lock (_writeLock)
                {
                    Stream.BeginWrite(message.Buffer, 0, message.Length, null, null);
                }
            }
            catch (ObjectDisposedException)
            {
                Close();
            }
        }

        public void Send(NetworkMessage message)
        {
            Send(message, true);
        }

        public void Send(NetworkMessage message, bool useEncryption, bool managementProtocol = false)
        {
            // if (isInTransaction)
            // {
            //    if (useEncryption == false)
            //        throw new Exception("Cannot send a packet without encryption as part of a transaction.");

            // transactionMessage.AddBytes(message.GetPacket());
            // }
            // else
            // {
            SendMessage(message, useEncryption, managementProtocol);
            // }
        }

        public void Close()
        {
            Stream.Close();
            Socket.Close();

            // Tells the subscribers of this event that this connection has been closed.
            OnCloseEvent?.Invoke(this);
        }

        public void SetXtea(uint[] xteaKey)
        {
            XTeaKey = xteaKey;
        }
    }
}
