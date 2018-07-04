// <copyright file="MoveItemPlayerAction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Actions
{
    using System;
    using System.Threading.Tasks;
    using OpenTibia.Communications.Packets.Incoming;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Movement;

    internal class MoveItemPlayerAction : BasePlayerAction
    {
        public MoveItemPlayerAction(IPlayer player, ItemMovePacket itemMovePacket, Location retryAtLocation)
            : base(player, itemMovePacket, retryAtLocation)
        {
        }

        protected override void InternalPerform()
        {
            var itemMovePacket = this.Packet as ItemMovePacket;

            if (itemMovePacket == null)
            {
                return;
            }

            switch (itemMovePacket.FromLocation.Type)
            {
                case LocationType.Ground:
                    this.MoveFromGround(itemMovePacket);
                    break;
                case LocationType.Container:
                    this.MoveFromContainer(itemMovePacket);
                    break;
                case LocationType.Slot:
                    this.MoveFromSlot(itemMovePacket);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Console.WriteLine($"Move {itemMovePacket.Count} {itemMovePacket.ClientId} from {itemMovePacket.FromLocation}:{itemMovePacket.FromStackPos} to {itemMovePacket.ToLocation}.");
        }

        private void MoveFromSlot(ItemMovePacket itemMovePacket)
        {
            var thing = this.Player.Inventory[(byte)itemMovePacket.FromLocation.Slot];

            var delayTime = TimeSpan.FromMilliseconds(200);
            IEvent movement = null;

            switch (itemMovePacket.ToLocation.Type)
            {
                case LocationType.Ground:
                    movement = new ThingMovementSlotToGround(this.Player.CreatureId, thing, itemMovePacket.FromLocation, itemMovePacket.ToLocation, itemMovePacket.Count);
                    break;
                case LocationType.Container:
                    movement = new ThingMovementSlotToContainer(this.Player.CreatureId, thing, itemMovePacket.FromLocation, itemMovePacket.ToLocation, itemMovePacket.Count);
                    break;
                case LocationType.Slot:
                    movement = new ThingMovementSlotToSlot(this.Player.CreatureId, thing, itemMovePacket.FromLocation, itemMovePacket.ToLocation, itemMovePacket.Count);
                    break;
            }

            // submit the movement.
            if (movement != null)
            {
                Game.Instance.ScheduleEvent(movement, delayTime);
            }
        }

        private void MoveFromContainer(ItemMovePacket itemMovePacket)
        {
            var container = this.Player.GetContainer(itemMovePacket.FromLocation.Container);
            var thing = container.Content[container.Content.Count - itemMovePacket.FromLocation.Z - 1];

            var delayTime = TimeSpan.FromMilliseconds(200);
            IEvent movement = null;

            switch (itemMovePacket.ToLocation.Type)
            {
                case LocationType.Ground:
                    movement = new ThingMovementContainerToGround(this.Player.CreatureId, thing, itemMovePacket.FromLocation, itemMovePacket.ToLocation, itemMovePacket.Count);
                    break;
                case LocationType.Container:
                    movement = new ThingMovementContainerToContainer(this.Player.CreatureId, thing, itemMovePacket.FromLocation, itemMovePacket.ToLocation, itemMovePacket.Count);
                    break;
                case LocationType.Slot:
                    movement = new ThingMovementContainerToSlot(this.Player.CreatureId, thing, itemMovePacket.FromLocation, itemMovePacket.ToLocation, itemMovePacket.Count);
                    break;
            }

            // submit the movement.
            if (movement != null)
            {
                Game.Instance.ScheduleEvent(movement, delayTime);
            }
        }

        private void MoveFromGround(ItemMovePacket itemMovePacket)
        {
            var fromTile = Game.Instance.GetTileAt(itemMovePacket.FromLocation);
            var thing = fromTile?.GetThingAtStackPosition(itemMovePacket.FromStackPos);

            var delayTime = TimeSpan.FromMilliseconds(200);
            IEvent movement = null;

            switch (itemMovePacket.ToLocation.Type)
            {
                case LocationType.Ground:
                    if (thing is ICreature)
                    {
                        delayTime = TimeSpan.FromSeconds(1);
                        movement = new CreatureMovementOnMap(this.Player.CreatureId, thing as ICreature, itemMovePacket.FromLocation, itemMovePacket.ToLocation);
                    }
                    else
                    {
                        movement = new ThingMovementOnMap(this.Player.CreatureId, thing, itemMovePacket.FromLocation, itemMovePacket.FromStackPos, itemMovePacket.ToLocation, itemMovePacket.Count);
                    }

                    break;
                case LocationType.Container:
                    movement = new ThingMovementGroundToContainer(this.Player.CreatureId, thing, itemMovePacket.FromLocation, itemMovePacket.FromStackPos, itemMovePacket.ToLocation, itemMovePacket.Count);
                    break;
                case LocationType.Slot:
                    movement = new ThingMovementGroundToSlot(this.Player.CreatureId, thing, itemMovePacket.FromLocation, itemMovePacket.FromStackPos, itemMovePacket.ToLocation, itemMovePacket.Count);
                    break;
            }

            // submit the movement.
            if (movement != null)
            {
                Game.Instance.ScheduleEvent(movement, delayTime);
            }
        }
    }
}