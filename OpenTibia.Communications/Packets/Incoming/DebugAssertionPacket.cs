// <copyright file="DebugAssertionPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public class DebugAssertionPacket : IPacketIncoming
    {
        public DebugAssertionPacket(NetworkMessage message)
        {
            this.AssertionLine = message.GetString();
            this.Date = message.GetString();
            this.Description = message.GetString();
            this.Comment = message.GetString();
        }

        public string AssertionLine { get; private set; }

        public string Date { get; private set; }

        public string Description { get; private set; }

        public string Comment { get; private set; }
    }
}
