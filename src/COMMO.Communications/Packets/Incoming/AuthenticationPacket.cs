// <copyright file="AuthenticationPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Incoming
{
    /// <summary>
    /// Class that represents an authentication packet.
    /// </summary>
    public class AuthenticationPacket : IPacketIncoming, IAuthenticationInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationPacket"/> class.
        /// </summary>
        /// <param name="message">The message to parse the packet from.</param>
        public AuthenticationPacket(NetworkMessage message)
        {
			Unknown = message.GetByte();
			Password = message.GetString();
			WorldName = message.GetString();
        }

        /// <inheritdoc/>
        public byte Unknown { get; private set; }

        /// <inheritdoc/>
        public string Password { get; private set; }

        /// <inheritdoc/>
        public string WorldName { get; private set; }
    }
}
