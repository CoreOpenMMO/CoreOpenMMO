// <copyright file="CreatePlayerListPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using COMMO.Data.Contracts;
using COMMO.Data.Models;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Incoming
{
    

    public class CreatePlayerListPacket : IPacketIncoming, IPlayerListInfo
    {
        public CreatePlayerListPacket(NetworkMessage message)
        {
            var count = message.GetUInt16();

			PlayerList = new List<IOnlinePlayer>();

            for (int i = 0; i < count; i++)
            {
				PlayerList.Add(new OnlinePlayer
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
