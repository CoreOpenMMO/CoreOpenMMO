// <copyright file="Connection.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications
{
    using System;
    using System.Net.Sockets;
    using OpenTibia.Security;
    using OpenTibia.Server.Data;

    public class Connection
    {
        private object writeLock;

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

        public string SourceIp => this.Socket?.RemoteEndPoint.ToString();

        public Connection()
        {
            this.writeLock = new object();
            this.Socket = null;
            this.Stream = null;
            this.InMessage = new NetworkMessage(0);
            this.XTeaKey = new uint[4];
            this.IsAuthenticated = false;
        }

        public void BeginStreamRead()
        {
            this.Stream.BeginRead(this.InMessage.Buffer, 0, 2, this.OnRead, null);
        }

        public void OnAccept(IAsyncResult ar)
        {
            if (ar == null)
            {
                throw new ArgumentNullException(nameof(ar));
            }

            this.Socket = ((TcpListener)ar.AsyncState).EndAcceptSocket(ar);
            this.Stream = new NetworkStream(this.Socket);

            if (SimpleDoSDefender.Instance.IsBlockedAddress(this.SourceIp))
            {
                // TODO: evaluate if it is worth just leaving the connection open but ignore it, so that they think they are successfully DoSing...
                // But we would need to think if it is a connection drain attack then...
                this.Close();
            }
            else
            {
                SimpleDoSDefender.Instance.LogConnectionAttempt(this.SourceIp);
                this.BeginStreamRead();
            }
        }

        private void OnRead(IAsyncResult ar)
        {
            if (!this.CompleteRead(ar))
            {
                return;
            }

            try
            {
                this.OnProcessEvent?.Invoke(this, this.InMessage);
                this.OnPostProcessEvent?.Invoke(this);
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
                int read = this.Stream.EndRead(ar);

                if (read == 0)
                {
                    // client disconnected
                    this.Close();
                    return false;
                }

                int size = BitConverter.ToUInt16(this.InMessage.Buffer, 0) + 2;

                while (read < size)
                {
                    if (this.Stream.CanRead)
                    {
                        read += this.Stream.Read(this.InMessage.Buffer, read, size - read);
                    }
                }

                this.InMessage.Resize(size);
                this.InMessage.GetUInt16(); // total length

                return true;
            }
            catch (Exception e)
            {
                // TODO: I must not swallow exceptions.
                // TODO: is closing the connection really necesary?
                Console.WriteLine(e.ToString());
                this.Close();
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
                message.PrepareToSend(this.XTeaKey);
            }
            else
            {
                message.PrepareToSendWithoutEncryption(managementProtocol);
            }

            try
            {
                lock (this.writeLock)
                {
                    this.Stream.BeginWrite(message.Buffer, 0, message.Length, null, null);
                }
            }
            catch (ObjectDisposedException)
            {
                this.Close();
            }
        }

        public void Send(NetworkMessage message)
        {
            this.Send(message, true);
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
            this.SendMessage(message, useEncryption, managementProtocol);
            // }
        }

        public void Close()
        {
            this.Stream.Close();
            this.Socket.Close();

            // Tells the subscribers of this event that this connection has been closed.
            this.OnCloseEvent?.Invoke(this);
        }

        public void SetXtea(uint[] xteaKey)
        {
            this.XTeaKey = xteaKey;
        }
    }
}
