// <copyright file="DefaultHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;
using COMMO.Communications;
using COMMO.Communications.Packets.Incoming;
using COMMO.Server.Data;

namespace COMMO.Server.Handlers
{
    internal class DefaultHandler : IncomingPacketHandler
    {
        public byte IncomingPacketType { get; }

        public DefaultHandler(byte incomingType)
        {
            IncomingPacketType = incomingType;
        }

        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var debugPacket = new DefaultReadPacket(message);

            var sb = new StringBuilder();

            foreach (var b in debugPacket.InfoBytes)
            {
                sb.AppendFormat("{0:x2} ", b);
            }

            Console.WriteLine($"Default handler received the following packet type: {IncomingPacketType}");
            Console.WriteLine(sb.ToString());
            Console.WriteLine();
        }
    }
}