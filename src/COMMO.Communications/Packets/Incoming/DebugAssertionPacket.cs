// <copyright file="DebugAssertionPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Communications.Packets.Incoming
{
    public class DebugAssertionPacket : IPacketIncoming
    {
        public DebugAssertionPacket(NetworkMessage message)
        {
			AssertionLine = message.GetString();
			Date = message.GetString();
			Description = message.GetString();
			Comment = message.GetString();
        }

        public string AssertionLine { get; private set; }

        public string Date { get; private set; }

        public string Description { get; private set; }

        public string Comment { get; private set; }
    }
}
