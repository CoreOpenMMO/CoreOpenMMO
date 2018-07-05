// <copyright file="Notification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using COMMO.Communications;
using COMMO.Communications.Interfaces;
using COMMO.Scheduling;
using COMMO.Scheduling.Contracts;
using COMMO.Server.Data;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Server.Notifications
{
    public abstract class Notification : BaseEvent, INotification
    {
        public Connection Connection { get; }

        public IList<IPacketOutgoing> ResponsePackets { get; protected set; }

        public Notification(Connection connection)
            : base(connection?.PlayerId ?? 0, EvaluationTime.OnSchedule)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            ResponsePackets = new List<IPacketOutgoing>();

            ActionsOnPass.Add(new GenericEventAction(Send));
        }

        public Notification(Connection connection, params IPacketOutgoing[] packets)
            : this(connection)
        {
            foreach (var packet in packets)
            {
                ResponsePackets.Add(packet);
            }
        }

        public abstract void Prepare();

        public void Send()
        {
            if (!ResponsePackets.Any())
            {
                return;
            }

            var networkMessage = new NetworkMessage(4);

            foreach (var packet in ResponsePackets)
            {
                networkMessage.AddPacket(packet);
            }

            Connection.Send(networkMessage);
            Console.WriteLine($"Sent {GetType().Name} [{EventId}] to {Connection.PlayerId}");

            // foreach (var packet in ResponsePackets)
            // {
            //    packet.CleanUp();
            // }
        }
    }
}
