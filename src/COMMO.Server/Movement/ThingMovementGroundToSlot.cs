// <copyright file="ThingMovementGroundToSlot.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using COMMO.Communications.Packets.Outgoing;
using COMMO.Data.Contracts;
using COMMO.Scheduling.Contracts;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;
using COMMO.Server.Events;
using COMMO.Server.Movement.EventConditions;
using COMMO.Server.Notifications;

namespace COMMO.Server.Movement
{
    internal class ThingMovementGroundToSlot : MovementBase
    {
        public ThingMovementGroundToSlot(uint creatureRequestingId, IThing thingMoving, Location fromLocation, byte fromStackPos, Location toLocation, byte count = 1)
            : base(creatureRequestingId, EvaluationTime.OnExecute)
        {
            if (count == 0 || count > 100)
            {
                throw new ArgumentException($"Invalid count {count}.", nameof(count));
            }

            FromLocation = fromLocation;
            FromStackPos = fromStackPos;
            FromTile = Game.Instance.GetTileAt(FromLocation);

            ToLocation = toLocation;
            ToSlot = (byte)toLocation.Slot;

            Thing = thingMoving;
            Count = count;

            var droppingItem = Requestor?.Inventory?[ToSlot];

            Conditions.Add(new SlotHasContainerAndContainerHasEnoughCapacityEventCondition(RequestorId, droppingItem));
            Conditions.Add(new GrabberHasEnoughCarryStrengthEventCondition(RequestorId, Thing, droppingItem));
            Conditions.Add(new ThingIsTakeableEventCondition(RequestorId, Thing));
            Conditions.Add(new LocationsMatchEventCondition(Thing?.Location ?? default, FromLocation));
            Conditions.Add(new TileContainsThingEventCondition(Thing, FromLocation, Count));

            ActionsOnPass.Add(new GenericEventAction(MoveFromGroudToSlot));
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
			if (FromTile == null || Thing == null || !(Thing is IItem updatedItem) || Requestor == null) {
				return;
			}

			var thingAtTile = FromTile.GetThingAtStackPosition(FromStackPos);

            if (thingAtTile == null)
            {
                return;
            }

            var thing = Thing;
            FromTile.RemoveThing(ref thing, Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, FromTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, FromTile.Location)), FromTile.Location);

            // and call any separation events.
            if (FromTile.HandlesSeparation) // TODO: what happens on separation of less than required quantity, etc?
            {
                foreach (var itemWithSeparation in FromTile.ItemsWithSeparation)
                {
                    var separationEvents = Game.Instance.EventsCatalog[ItemEventType.Separation].Cast<SeparationItemEvent>();

                    var candidate = separationEvents.FirstOrDefault(e => e.ThingIdOfSeparation == itemWithSeparation.Type.TypeId && e.Setup(itemWithSeparation, Requestor) && e.CanBeExecuted);

                    // Execute all actions.
                    candidate?.Execute();
                }
            }

            if (thing != Thing)
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
			if (!Requestor.Inventory.Add(updatedItem, out var addedItem, ToSlot, updatedItem.Count)) {
				// failed to add to the slot, add again to the source tile
				FromTile.AddThing(ref thing, thing.Count);

				// notify all spectator players of that tile.
				Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, FromTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, FromTile.Location)), FromTile.Location);

				// call any collision events again.
				if (FromTile.HandlesCollision) {
					foreach (var itemWithCollision in FromTile.ItemsWithCollision) {
						var collisionEvents = Game.Instance.EventsCatalog[ItemEventType.Collision].Cast<CollisionItemEvent>();

						var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, updatedItem) && e.CanBeExecuted);

						// Execute all actions.
						candidate?.Execute();
					}
				}
			}
			else {
				// added the new item to the slot
				if (addedItem == null) {
					return;
				}

				// we exchanged or got some leftover item, place back in the source container at any index.
				IThing remainderThing = addedItem;

				FromTile.AddThing(ref remainderThing, remainderThing.Count);

				// notify all spectator players of that tile.
				Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, FromTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, FromTile.Location)), FromTile.Location);

				// call any collision events again.
				if (!FromTile.HandlesCollision) {
					return;
				}

				foreach (var itemWithCollision in FromTile.ItemsWithCollision) {
					var collisionEvents = Game.Instance.EventsCatalog[ItemEventType.Collision].Cast<CollisionItemEvent>();

					var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, remainderThing) && e.CanBeExecuted);

					// Execute all actions.
					candidate?.Execute();
				}
			}
		}
    }
}