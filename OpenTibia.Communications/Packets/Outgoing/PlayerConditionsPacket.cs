// <copyright file="PlayerConditionsPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class PlayerConditionsPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.PlayerConditions;

        public IPlayer Player { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(this.PacketType);

            message.AddByte(0x00);
        }

        public override void CleanUp()
        {
            this.Player = null;
        }
    }
}
