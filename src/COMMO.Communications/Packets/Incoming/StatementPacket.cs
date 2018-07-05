// <copyright file="StatementPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Incoming
{
    public class StatementPacket : IPacketIncoming, IStatementInfo
    {
        public StatementPacket(NetworkMessage message)
        {
			Unknown = message.GetUInt32();
			StatementId = message.GetUInt32();
			Count = message.GetUInt16();

			Data = new List<Tuple<uint, uint, string, string>>();

            for (int i = 0; i < Count; i++)
            {
                message.GetUInt32(); // ignore, this is the same statementId

				// timestamp, playerId, channel, message
				Data.Add(new Tuple<uint, uint, string, string>(message.GetUInt32(), message.GetUInt32(), message.GetString(), message.GetString()));
            }
        }

        public uint Unknown { get; set; }

        public uint StatementId { get; set; }

        public ushort Count { get; set; }

        public IList<Tuple<uint, uint, string, string>> Data { get; set; }
    }
}
