// <copyright file="OpenTibiaListener.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using OpenTibia.Communications.Interfaces;

    public abstract class OpenTibiaListener : TcpListener, IOpenTibiaListener
    {
        public IProtocol Protocol { get; }

        public int Port { get; }

        protected ICollection<Connection> Connections { get; }

        protected OpenTibiaListener(int port, IProtocol protocol)
            : base(IPAddress.Any, port)
        {
            this.Protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));

            this.Port = port;
            this.Connections = new HashSet<Connection>();
        }

        public void BeginListening()
        {
            this.Start();
            this.BeginAcceptSocket(this.OnAccept, this);
        }

        public void EndListening()
        {
            try
            {
                this.Stop();
            }
            catch (SocketException socEx)
            {
                // TODO: proper logging.
                Console.WriteLine(socEx.ToString());
            }
        }

        public void OnAccept(IAsyncResult ar)
        {
            Connection connection = new Connection();

            connection.OnCloseEvent += this.OnConnectionClose;
            connection.OnProcessEvent += this.Protocol.ProcessMessage;
            connection.OnPostProcessEvent += this.Protocol.PostProcessMessage;

            this.Connections.Add(connection);
            this.Protocol.OnAcceptNewConnection(connection, ar);

            this.BeginAcceptSocket(this.OnAccept, this);
        }

        private void OnConnectionClose(Connection connection)
        {
            // De-subscribe to this event first.
            connection.OnCloseEvent -= this.OnConnectionClose;
            connection.OnProcessEvent -= this.Protocol.ProcessMessage;
            connection.OnPostProcessEvent -= this.Protocol.PostProcessMessage;

            this.Connections.Remove(connection);
        }
    }
}
