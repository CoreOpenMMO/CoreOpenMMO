// <copyright file="IncomingPacketHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using COMMO.Communications;
using COMMO.Communications.Interfaces;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Server.Handlers
{
    public abstract class IncomingPacketHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; protected set; }

        protected IncomingPacketHandler()
        {
            ResponsePackets = new List<IPacketOutgoing>();
        }

        public abstract void HandleMessageContents(NetworkMessage message, Connection connection);
    }
}