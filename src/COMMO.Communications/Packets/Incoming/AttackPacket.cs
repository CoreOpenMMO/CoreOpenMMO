// <copyright file="AttackPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Incoming
{
    /// <summary>
    /// Class that represents and attack packet.
    /// </summary>
    public class AttackPacket : IPacketIncoming, IAttackInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackPacket"/> class.
        /// </summary>
        /// <param name="message">The message to parse the packet from.</param>
        public AttackPacket(NetworkMessage message)
        {
			CreatureId = message.GetUInt32();
        }

        /// <inheritdoc/>
        public uint CreatureId { get; }
    }
}
