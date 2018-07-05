﻿// <copyright file="AuthenticationHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Handlers.Management
{
    using COMMO.Common.Helpers;
    using COMMO.Communications;
    using COMMO.Communications.Packets.Incoming;
    using COMMO.Communications.Packets.Outgoing;
    using COMMO.Configuration;
    using COMMO.Server.Data;

    /// <summary>
    /// Class that represents an authentication request handler for the management service
    /// </summary>
    internal class AuthenticationHandler : IncomingPacketHandler
    {
        /// <inheritdoc/>
        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            connection.ThrowIfNull(nameof(connection));

            var authPacket = new AuthenticationPacket(message);

            var result = authPacket.Password.Equals(ServiceConfiguration.GetConfiguration().QueryManagerPassword);

            connection.IsAuthenticated = result;

            this.ResponsePackets.Add(new AuthenticationResultPacket
            {
                HadError = !result
            });
        }
    }
}