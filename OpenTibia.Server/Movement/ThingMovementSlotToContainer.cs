// <copyright file="ThingMovementSlotToContainer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement
{
    using System;
    using System.Linq;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Movement.EventConditions;
    using OpenTibia.Server.Notifications;

    internal class ThingMovementSlotToContainer : MovementBase
    {
        public ThingMovementSlotToContainer(uint requestorId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base(requestorId, EvaluationTime.OnExecute)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(count));
            }

            if (this.Requestor == null)
            {
                throw new ArgumentException("Invalid requestor id.", nameof(requestorId));
            }

            this.FromLocation = fromLocation;
            this.FromSlot = (byte)this.FromLocation.Slot;

            this.ToLocation = toLocation;
            this.ToContainer = (this.Requestor as IPlayer)?.GetContainer(this.ToLocation.Container);
            this.ToIndex = (byte)this.ToLocation.Z;

            this.Item = thingMoving as IItem;
            this.Count = count;

            this.Conditions.Add(new SlotContainsItemAndCountEventCondition(this.RequestorId, this.Item, this.FromSlot, this.Count));
            this.Conditions.Add(new GrabberHasContainerOpenEventCondition(this.RequestorId, this.ToContainer));
            this.Conditions.Add(new ContainerHasEnoughCapacityEventCondition(this.ToContainer));

            this.ActionsOnPass.Add(new GenericEventAction(this.MoveSlotToContainer));
        }

        public Location FromLocation { get; }

        public byte FromSlot { get; }

        public Location ToLocation { get; }

        public IContainer ToContainer { get; }

        public byte ToIndex { get; }

        public IItem Item { get; }

        public byte Count { get; }

        private void MoveSlotToContainer()
        {
            if (this.Item == null || this.Requestor == null)
            {
                return;
            }

            bool partialRemove;

            // attempt to remove the item from the inventory
            var movingItem = this.Requestor.Inventory?.Remove(this.FromSlot, this.Count, out partialRemove);

            if (movingItem == null)
            {
                return;
            }

            // successfully removed thing from the source container
            // attempt to add to the dest container
            if (this.ToContainer.AddContent(movingItem, this.ToIndex))
            {
                return;
            }

            // failed to add to the slot, add again to the source slot
            if (!this.Requestor.Inventory.Add(movingItem, out movingItem, this.FromSlot, movingItem.Count))
            {
                // and we somehow failed to re-add it to the source container...
                // throw to the ground.
                IThing thing = movingItem;
                this.Requestor.Tile.AddThing(ref thing, movingItem.Count);

                // notify all spectator players of that tile.
                Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, this.Requestor.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, this.Requestor.Location)), this.Requestor.Location);

                // call any collision events again.
                if (this.Requestor.Tile.HandlesCollision)
                {
                    foreach (var itemWithCollision in this.Requestor.Tile.ItemsWithCollision)
                    {
                        var collisionEvents = Game.Instance.EventsCatalog[ItemEventType.Collision].Cast<CollisionItemEvent>();

                        var candidate =
                            collisionEvents.FirstOrDefault(
                                e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId &&
                                     e.Setup(itemWithCollision, thing) && e.CanBeExecuted);

                        // Execute all actions.
                        candidate?.Execute();
                    }
                }
            }
        }
    }
}