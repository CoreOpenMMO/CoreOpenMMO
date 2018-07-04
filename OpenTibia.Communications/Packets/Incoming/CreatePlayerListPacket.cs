// <copyright file="CreatePlayerListPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using System.Collections.Generic;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Data.Models;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class CreatePlayerListPacket : IPacketIncoming, IPlayerListInfo
    {
        public CreatePlayerListPacket(NetworkMessage message)
        {
            var count = message.GetUInt16();

            this.PlayerList = new List<IOnlinePlayer>();

            for (int i = 0; i < count; i++)
            {
                this.PlayerList.Add(new OnlinePlayer
                {
                    Name = message.GetString(),
                    Level = message.GetUInt16(),
                    Vocation = message.GetString()
                });
            }
        }

        public IList<IOnlinePlayer> PlayerList { get; set; }
    }
}
