// <copyright file="AccountLoginPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    /// <summary>
    /// Class that represents an account login packet.
    /// </summary>
    public class AccountLoginPacket : IPacketIncoming, IAccountLoginInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountLoginPacket"/> class.
        /// </summary>
        /// <param name="message">The message to parse the packet from.</param>
        public AccountLoginPacket(NetworkMessage message)
        {
            this.XteaKey = new uint[]
            {
                message.GetUInt32(),
                message.GetUInt32(),
                message.GetUInt32(),
                message.GetUInt32()
            };

            this.AccountNumber = message.GetUInt32();
            this.Password = message.GetString();
        }

        /// <inheritdoc/>
        public uint AccountNumber { get; }

        /// <inheritdoc/>
        public string Password { get; }

        /// <inheritdoc/>
        public uint[] XteaKey { get; }
    }
}
