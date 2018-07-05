// <copyright file="CharacterDeathPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Incoming
{
    /// <summary>
    /// Class that represents a character death packet.
    /// </summary>
    public class CharacterDeathPacket : IPacketIncoming, ICharacterDeathInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterDeathPacket"/> class.
        /// </summary>
        /// <param name="message">The message to parse the packet from.</param>
        public CharacterDeathPacket(NetworkMessage message)
        {
			VictimId = (int)message.GetUInt32();
			VictimLevel = (short)message.GetUInt16();
			KillerId = (int)message.GetUInt32();
			KillerName = message.GetString();
			Unjustified = message.GetByte() > 0;
			Timestamp = DateTime.FromFileTimeUtc(message.GetUInt32());
        }

        /// <inheritdoc/>
        public int VictimId { get; }

        /// <inheritdoc/>
        public short VictimLevel { get; }

        /// <inheritdoc/>
        public int KillerId { get; }

        /// <inheritdoc/>
        public string KillerName { get; }

        /// <inheritdoc/>
        public bool Unjustified { get; }

        /// <inheritdoc/>
        public DateTime Timestamp { get; }
    }
}
