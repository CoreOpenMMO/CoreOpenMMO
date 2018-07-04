// <copyright file="LoadPlayersResultPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using System.Collections.Generic;
    using OpenTibia.Data.Models;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class LoadPlayersResultPacket : PacketOutgoing
    {
        public IList<PlayerModel> LoadedPlayers { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.NoType;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(0x00);
            message.AddUInt32((uint)this.LoadedPlayers.Count);

            foreach (var player in this.LoadedPlayers)
            {
                message.AddString(player.Charname);
                message.AddUInt32((uint)player.Account_Id);
            }
        }

        public override void CleanUp()
        {
            this.LoadedPlayers = null;
        }
    }
}