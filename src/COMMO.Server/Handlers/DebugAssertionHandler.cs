// <copyright file="DebugAssertionHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Handlers
{
    using System;
    using COMMO.Communications;
    using COMMO.Communications.Packets.Incoming;
    using COMMO.Server.Data;

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