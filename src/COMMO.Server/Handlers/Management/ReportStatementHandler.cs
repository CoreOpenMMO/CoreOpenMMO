// <copyright file="ReportStatementHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using COMMO.Communications;
using COMMO.Communications.Interfaces;
using COMMO.Communications.Packets.Incoming;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Server.Handlers.Management
{
    internal class ReportStatementHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var ruleViolationPacket = new RuleViolationPacket(message);
            var statementPacket = new StatementPacket(message);

            // TODO: log somewhere? :)
            ResponsePackets.Add(new DefaultNoErrorPacket());
        }
    }
}