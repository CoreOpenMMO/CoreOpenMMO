// <copyright file="LookAtHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Handlers
{
    using System;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Packets.Incoming;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data;
    using OpenTibia.Server.Data.Interfaces;

    internal class LookAtHandler : IncomingPacketHandler
    {
        public override void HandleMessageContents(NetworkMessage message, Connection connection)
        {
            var lookAtPacket = new LookAtPacket(message);
            IThing thing = null;
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            if (player == null)
            {
                return;
            }

            Console.WriteLine($"LookAt {lookAtPacket.ThingId}.");

            if (lookAtPacket.Location.Type != LocationType.Ground || player.CanSee(lookAtPacket.Location))
            {
                // Get thing at location
                switch (lookAtPacket.Location.Type)
                {
                    case LocationType.Ground:
                        thing = Game.Instance.GetTileAt(lookAtPacket.Location).GetThingAtStackPosition(lookAtPacket.StackPosition);
                        break;
                    case LocationType.Container:
                        // TODO: implement containers.
                        // Container container = player.Inventory.GetContainer(location.Container);
                        // if (container != null)
                        // {
                        //    return container.GetItem(location.ContainerPosition);
                        // }
                        break;
                    case LocationType.Slot:
                        thing = player.Inventory[(byte)lookAtPacket.Location.Slot];
                        break;
                }

                if (thing != null)
                {
                    this.ResponsePackets.Add(new TextMessagePacket
                    {
                        Type = MessageType.DescriptionGreen,
                        Message = $"You see {thing.InspectionText}."
                    });
                }
            }
        }
    }
}