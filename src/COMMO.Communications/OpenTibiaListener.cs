// <copyright file="OpenTibiaListener.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using COMMO.Communications.Interfaces;

namespace COMMO.Communications
{
    public abstract class OpenTibiaListener : TcpListener, IOpenTibiaListener
    {
        public IProtocol Protocol { get; }

        public int Port { get; }

		private OpenTibiaProtocolType _openTibiaProtocolType;

        protected ICollection<Connection> _connections { get; }

        protected OpenTibiaListener(int port, IProtocol protocol, OpenTibiaProtocolType openTibiaProtocolType)
            : base(IPAddress.Any, port)
        {
            Protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));

			_openTibiaProtocolType = openTibiaProtocolType;
            Port = port;
            _connections = new HashSet<Connection>();
        }

        public void BeginListening()
        {
            Start();
            BeginAcceptSocket(OnAccept, this);
        }

        public void EndListening()
        {
            try
            {
                Stop();
            }
            catch (SocketException socEx)
            {
                // TODO: proper logging.
                Console.WriteLine(socEx.ToString());
            }
        }

        public void OnAccept(IAsyncResult ar)
        {
            var connection = new Connection(_openTibiaProtocolType);

            connection.OnCloseEvent += OnConnectionClose;
            connection.OnProcessEvent += Protocol.ProcessMessage;
            connection.OnPostProcessEvent += Protocol.PostProcessMessage;

            _connections.Add(connection);
            Protocol.OnAcceptNewConnection(connection, ar);

            BeginAcceptSocket(OnAccept, this);
        }

        private void OnConnectionClose(Connection connection)
        {
            // De-subscribe to this event first.
            connection.OnCloseEvent -= OnConnectionClose;
            connection.OnProcessEvent -= Protocol.ProcessMessage;
            connection.OnPostProcessEvent -= Protocol.PostProcessMessage;

            _connections.Remove(connection);
        }
    }
}