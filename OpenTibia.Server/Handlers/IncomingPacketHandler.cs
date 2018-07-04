// <copyright file="IncomingPacketHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers
{
    using System.Collections.Generic;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Interfaces;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    public abstract class IncomingPacketHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; protected set; }

        protected IncomingPacketHandler()
        {
            this.ResponsePackets = new List<IPacketOutgoing>();
        }

        public abstract void HandleMessageContents(NetworkMessage message, Connection connection);
    }
}