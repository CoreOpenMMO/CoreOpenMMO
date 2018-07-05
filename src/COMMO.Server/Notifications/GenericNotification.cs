﻿// <copyright file="GenericNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using COMMO.Communications;
    using COMMO.Communications.Packets.Outgoing;

    internal class GenericNotification : Notification
    {
        public IEnumerable<PacketOutgoing> OutgoingPackets { get; }

        public GenericNotification(Connection connection, params PacketOutgoing[] outgoingPackets)
            : base(connection)
        {
            if (outgoingPackets == null || !outgoingPackets.Any())
            {
                throw new ArgumentNullException(nameof(outgoingPackets));
            }

            OutgoingPackets = outgoingPackets;
        }

        public override void Prepare()
        {
            foreach (var packet in OutgoingPackets)
            {
                ResponsePackets.Add(packet);
            }
        }
    }
}