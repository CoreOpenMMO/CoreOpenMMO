// <copyright file="IIncomingPacketHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Interfaces
{
    using System.Collections.Generic;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    /// <summary>
    /// Interface for an incomming packet handler.
    /// </summary>
    public interface IIncomingPacketHandler
    {
        /// <summary>
        /// Gets a collection of <see cref="IPacketOutgoing"/> packets readily available to be sent as response when this message was handled.
        /// </summary>
        /// <remarks>The handler chooses to add or not contents to this collection, which could also be null.</remarks>
        IList<IPacketOutgoing> ResponsePackets { get; }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        void HandleMessageContents(NetworkMessage message, Connection connection);
    }
}
