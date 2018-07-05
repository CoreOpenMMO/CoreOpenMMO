// <copyright file="BasePlayerAction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Actions
{
    using System.Collections.Generic;
    using System.Linq;
    using COMMO.Common.Helpers;
    using COMMO.Communications.Packets.Outgoing;
    using COMMO.Server.Data.Interfaces;
    using COMMO.Server.Data.Models.Structs;
    using COMMO.Server.Notifications;

    internal abstract class BasePlayerAction : IAction
    {
        public IPlayer Player { get; }

        public IPacketIncoming Packet { get; }

        public Location RetryLocation { get; }

        public IList<IPacketOutgoing> ResponsePackets { get; }

        protected BasePlayerAction(IPlayer player, IPacketIncoming packet, Location retryLocation)
        {
            player.ThrowIfNull(nameof(player));
            packet.ThrowIfNull(nameof(packet));

            Player = player;
            Packet = packet;
            RetryLocation = retryLocation;
            ResponsePackets = new List<IPacketOutgoing>();
        }

        public void Perform()
        {
            InternalPerform();

            if (ResponsePackets.Any())
            {
                Game.Instance.NotifySinglePlayer(Player, conn => new GenericNotification(conn, ResponsePackets.Cast<PacketOutgoing>().ToArray()));
            }
        }

        protected abstract void InternalPerform();
    }
}