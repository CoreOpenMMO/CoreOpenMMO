// <copyright file="DebugAssertionHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers
{
    using System;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Packets.Incoming;
    using OpenTibia.Server.Data;

    internal class DebugAssertionHandler : IncomingPacketHandler
    {
        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var packet = new DebugAssertionPacket(message);

            Console.WriteLine($"{packet.AssertionLine}");
            Console.WriteLine($"{packet.Date}");
            Console.WriteLine($"{packet.Description}");
            Console.WriteLine($"{packet.Comment}");
        }
    }
}