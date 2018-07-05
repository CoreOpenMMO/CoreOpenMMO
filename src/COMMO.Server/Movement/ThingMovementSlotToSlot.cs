// <copyright file="ThingMovementSlotToSlot.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using COMMO.Data.Contracts;
using COMMO.Scheduling.Contracts;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;
using COMMO.Server.Events;
using COMMO.Server.Movement.EventConditions;
using COMMO.Server.Notifications;

namespace COMMO.Server.Movement
{
    internal class ThingMovementSlotToSlot : MovementBase
    {
        public ThingMovementSlotToSlot(uint requestorId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base(requestorId, EvaluationTime.OnExecute)
        {
            // intentionally left thing null check out. Handled by Perform().
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            FromLocation = fromLocation;
            FromSlot = (byte)FromLocation.Slot;

            ToLocation = toLocation;
            ToSlot = (byte)ToLocation.Slot;

            Item = thingMoving as IItem;
            Count = count;

            Conditions.Add(new SlotContainsItemAndCountEventCondition(requestorId, Item, FromSlot, Count));

            ActionsOnPass.Add(new GenericEventAction(MoveBetweenSlots));
        }

        public Location FromLocation { get; }

        public byte FromSlot { get; }

        public Location ToLocation { get; }

        public byte ToSlot { get; }

        public IItem Item { get; }

        public byte Count { get; }

        private void MoveBetweenSlots()
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

            // attempt to place the intended item at the slot.
            IItem addedItem;
            if (!Requestor.Inventory.Add(movingItem, out addedItem, ToSlot, movingItem.Count))
            {
                // failed to add to the slot, add again to the source slot
                if (!Requestor.Inventory.Add(movingItem, out addedItem, FromSlot, movingItem.Count))
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
            else
            {
                if (addedItem == null)
                {
                    return;
                }

                // added the new item to the slot
                IItem extraAddedItem;
                if (!Requestor.Inventory.Add(addedItem, out extraAddedItem, FromSlot, movingItem.Count))
                {
                    // we exchanged or got some leftover item, place back in the source container at any index.
                    IThing remainderThing = extraAddedItem;

                    Requestor.Tile.AddThing(ref remainderThing, remainderThing.Count);

                    // notify all spectator players of that tile.
                    Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, Requestor.Tile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, Requestor.Location)), Requestor.Location);

                    // call any collision events again.
                    if (!Requestor.Tile.HandlesCollision)
                    {
                        return;
                    }

                    foreach (var itemWithCollision in Requestor.Tile.ItemsWithCollision)
                    {
                        var collisionEvents = Game.Instance.EventsCatalog[ItemEventType.Collision].Cast<CollisionItemEvent>();

                        var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, remainderThing) && e.CanBeExecuted);

                        // Execute all actions.
                        candidate?.Execute();
                    }
                }
            }
        }
    }
}