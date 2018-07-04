// <copyright file="InsertHousesHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers.Management
{
    using System.Collections.Generic;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Interfaces;
    using OpenTibia.Communications.Packets.Incoming;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    internal class InsertHousesHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var inserHousePacket = new InsertHousePacket(message);

            // TODO: actually update house info?
            // using (OpenTibiaDbContext otContext = new OpenTibiaDbContext())
            // {
            this.ResponsePackets.Add(new DefaultNoErrorPacket());
            // }

            // return new DefaultErrorPacket();
        }
    }
}