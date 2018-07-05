// <copyright file="ThingMovementSlotToContainer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Movement
{
    using System;
    using System.Linq;
    using COMMO.Data.Contracts;
    using COMMO.Scheduling.Contracts;
    using COMMO.Server.Data.Interfaces;
    using COMMO.Server.Data.Models.Structs;
    using COMMO.Server.Events;
    using COMMO.Server.Movement.EventConditions;
    using COMMO.Server.Notifications;

    internal class ThingMovementSlotToContainer : MovementBase
    {
        public ThingMovementSlotToContainer(uint requestorId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base(requestorId, EvaluationTime.OnExecute)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(count));
            }

            if (Requestor == null)
            {
                throw new ArgumentException("Invalid requestor id.", nameof(requestorId));
            }

            FromLocation = fromLocation;
            FromSlot = (byte)FromLocation.Slot;

            ToLocation = toLocation;
            ToContainer = (Requestor as IPlayer)?.GetContainer(ToLocation.Container);
            ToIndex = (byte)ToLocation.Z;

            Item = thingMoving as IItem;
            Count = count;

            Conditions.Add(new SlotContainsItemAndCountEventCondition(RequestorId, Item, FromSlot, Count));
            Conditions.Add(new GrabberHasContainerOpenEventCondition(RequestorId, ToContainer));
            Conditions.Add(new ContainerHasEnoughCapacityEventCondition(ToContainer));

            ActionsOnPass.Add(new GenericEventAction(MoveSlotToContainer));
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
            if (Item == null || Requestor == null)
            {
                return;
            }

            bool partialRemove;

            // attempt to remove the item from the inventory
            var movingItem = Requestor.Inventory?.Remove(FromSlot, Count, out partialRemove);

            if (movingItem == null)
            {
                return;
            }

            // successfully removed thing from the source container
            // attempt to add to the dest container
            if (ToContainer.AddContent(movingItem, ToIndex))
            {
                return;
            }

            // failed to add to the slot, add again to the source slot
            if (!Requestor.Inventory.Add(movingItem, out movingItem, FromSlot, movingItem.Count))
            {
                // and we somehow failed to re-add it to the source container...
                // throw to the ground.
                IThing thing = movingItem;
                Requestor.Tile.AddThing(ref thing, movingItem.Count);

                // notify all spectator players of that tile.
                Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, Requestor.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, Requestor.Location)), Requestor.Location);

                // call any collision events again.
                if (Requestor.Tile.HandlesCollision)
                {
                    foreach (var itemWithCollision in Requestor.Tile.ItemsWithCollision)
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