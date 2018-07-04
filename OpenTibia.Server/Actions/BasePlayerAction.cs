// <copyright file="BasePlayerAction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Actions
{
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Notifications;

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

            this.Player = player;
            this.Packet = packet;
            this.RetryLocation = retryLocation;
            this.ResponsePackets = new List<IPacketOutgoing>();
        }

        public void Perform()
        {
            this.InternalPerform();

            if (this.ResponsePackets.Any())
            {
                Game.Instance.NotifySinglePlayer(this.Player, conn => new GenericNotification(conn, this.ResponsePackets.Cast<PacketOutgoing>().ToArray()));
            }
        }

        protected abstract void InternalPerform();
    }
}