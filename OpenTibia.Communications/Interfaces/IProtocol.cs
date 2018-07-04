// <copyright file="IProtocol.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Interfaces
{
    using System;
    using OpenTibia.Server.Data;

    /// <summary>
    /// Common interface for protocols.
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// Gets a value indicating whether the protocol should keep the connection open after recieving a packet.
        /// </summary>
        bool KeepConnectionOpen { get; }

        /// <summary>
        /// Handles a new connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="ar">The result of connecting.</param>
        void OnAcceptNewConnection(Connection connection, IAsyncResult ar);

        /// <summary>
        /// Processes an incomming message from the connection.
        /// </summary>
        /// <param name="connection">The connection where the message is being read from.</param>
        /// <param name="inboundMessage">The message to process.</param>
        void ProcessMessage(Connection connection, NetworkMessage inboundMessage);

        /// <summary>
        /// Runs after processing a message from the connection.
        /// </summary>
        /// <param name="connection">The connection where the message is from.</param>
        void PostProcessMessage(Connection connection);
    }
}
