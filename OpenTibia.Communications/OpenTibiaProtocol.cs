// <copyright file="OpenTibiaProtocol.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications
{
    using System;
    using OpenTibia.Communications.Interfaces;
    using OpenTibia.Server.Data;

    public abstract class OpenTibiaProtocol : IProtocol
    {
        public virtual bool KeepConnectionOpen { get; protected set; }

        public IHandlerFactory HandlerFactory { get; protected set; }

        protected OpenTibiaProtocol(IHandlerFactory handlerFactory)
        {
            if (handlerFactory == null)
            {
                throw new ArgumentNullException(nameof(handlerFactory));
            }

            this.HandlerFactory = handlerFactory;
        }

        public virtual void OnAcceptNewConnection(Connection connection, IAsyncResult ar)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            connection.OnAccept(ar);
        }

        public virtual void PostProcessMessage(Connection connection)
        {
            if (!this.KeepConnectionOpen)
            {
                connection.Close();
            }
            else if (connection.Stream != null)
            {
                connection.InMessage.Reset();
                connection.BeginStreamRead();
            }
        }

        public abstract void ProcessMessage(Connection connection, NetworkMessage inboundMessage);
    }
}
