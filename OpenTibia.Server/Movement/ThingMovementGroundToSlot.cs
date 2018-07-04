// <copyright file="ThingMovementGroundToSlot.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement
{
    using System;
    using System.Linq;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Movement.EventConditions;
    using OpenTibia.Server.Notifications;

    internal class ThingMovementGroundToSlot : MovementBase
    {
        public ThingMovementGroundToSlot(uint creatureRequestingId, IThing thingMoving, Location fromLocation, byte fromStackPos, Location toLocation, byte count = 1)
            : base(creatureRequestingId, EvaluationTime.OnExecute)
        {
            if (count == 0 || count > 100)
            {
                throw new ArgumentException($"Invalid count {count}.", nameof(count));
            }

            this.FromLocation = fromLocation;
            this.FromStackPos = fromStackPos;
            this.FromTile = Game.Instance.GetTileAt(this.FromLocation);

            this.ToLocation = toLocation;
            this.ToSlot = (byte)toLocation.Slot;

            this.Thing = thingMoving;
            this.Count = count;

            var droppingItem = this.Requestor?.Inventory?[this.ToSlot];

            this.Conditions.Add(new SlotHasContainerAndContainerHasEnoughCapacityEventCondition(this.RequestorId, droppingItem));
            this.Conditions.Add(new GrabberHasEnoughCarryStrengthEventCondition(this.RequestorId, this.Thing, droppingItem));
            this.Conditions.Add(new ThingIsTakeableEventCondition(this.RequestorId, this.Thing));
            this.Conditions.Add(new LocationsMatchEventCondition(this.Thing?.Location ?? default(Location), this.FromLocation));
            this.Conditions.Add(new TileContainsThingEventCondition(this.Thing, this.FromLocation, this.Count));

            this.ActionsOnPass.Add(new GenericEventAction(this.MoveFromGroudToSlot));
        }

        public Location FromLocation { get; }

        public byte FromStackPos { get; }

        public ITile FromTile { get; }

        public Location ToLocation { get; }

        public byte ToSlot { get; }

        public IThing Thing { get; }

        public byte Count { get; }

        private void MoveFromGroudToSlot()
        {
            var updatedItem = this.Thing as IItem;

            if (this.FromTile == null || this.Thing == null || updatedItem == null || this.Requestor == null)
            {
                return;
            }

            var thingAtTile = this.FromTile.GetThingAtStackPosition(this.FromStackPos);

            if (thingAtTile == null)
            {
                return;
            }

            var thing = this.Thing;
            this.FromTile.RemoveThing(ref thing, this.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, this.FromTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, this.FromTile.Location)), this.FromTile.Location);

            // and call any separation events.
            if (this.FromTile.HandlesSeparation) // TODO: what happens on separation of less than required quantity, etc?
            {
                foreach (var itemWithSeparation in this.FromTile.ItemsWithSeparation)
                {
                    var separationEvents = Game.Instance.EventsCatalog[ItemEventType.Separation].Cast<SeparationItemEvent>();

                    var candidate = separationEvents.FirstOrDefault(e => e.ThingIdOfSeparation == itemWithSeparation.Type.TypeId && e.Setup(itemWithSeparation, this.Requestor) && e.CanBeExecuted);

                    // Execute all actions.
                    candidate?.Execute();
                }
            }

            if (thing != this.Thing)
            {
                // item got split cause we removed less than the total amount.
                // update the thing we're adding to the container.
                updatedItem = thing as IItem;
            }

            if (updatedItem == null)
            {
                return;
            }

            // attempt to place the intended item at the slot.
            IItem addedItem;
            if (!this.Requestor.Inventory.Add(updatedItem, out addedItem, this.ToSlot, updatedItem.Count))
            {
                // failed to add to the slot, add again to the source tile
                this.FromTile.AddThing(ref thing, thing.Count);

                // notify all spectator players of that tile.
                Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, this.FromTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, this.FromTile.Location)), this.FromTile.Location);

                // call any collision events again.
                if (this.FromTile.HandlesCollision)
                {
                    foreach (var itemWithCollision in this.FromTile.ItemsWithCollision)
                    {
                        var collisionEvents = Game.Instance.EventsCatalog[ItemEventType.Collision].Cast<CollisionItemEvent>();

                        var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, updatedItem) && e.CanBeExecuted);

                        // Execute all actions.
                        candidate?.Execute();
                    }
                }
            }
            else
            {
                // added the new item to the slot
                if (addedItem == null)
                {
                    return;
                }

                // we exchanged or got some leftover item, place back in the source container at any index.
                IThing remainderThing = addedItem;

                this.FromTile.AddThing(ref remainderThing, remainderThing.Count);

                // notify all spectator players of that tile.
                Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, this.FromTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, this.FromTile.Location)), this.FromTile.Location);

                // call any collision events again.
                if (!this.FromTile.HandlesCollision)
                {
                    return;
                }

                foreach (var itemWithCollision in this.FromTile.ItemsWithCollision)
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