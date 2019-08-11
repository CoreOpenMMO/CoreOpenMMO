// <copyright file="ThingMovementSlotToGround.cs" company="2Dudes">
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
    internal class ThingMovementSlotToGround : MovementBase
    {
        public ThingMovementSlotToGround(uint creatureRequestingId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base(creatureRequestingId, EvaluationTime.OnExecute)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(count));
            }

            FromLocation = fromLocation;
            FromSlot = (byte)fromLocation.Slot;

            ToLocation = toLocation;
            ToTile = Game.Instance.GetTileAt(ToLocation);

            Item = thingMoving as IItem;
            Count = count;

            if (Requestor != null)
            {
                Conditions.Add(new CanThrowBetweenEventCondition(RequestorId, Requestor.Location, ToLocation));
            }

            Conditions.Add(new SlotContainsItemAndCountEventCondition(creatureRequestingId, Item, FromSlot, Count));
            Conditions.Add(new LocationNotObstructedEventCondition(RequestorId, Item, ToLocation));
            Conditions.Add(new LocationHasTileWithGroundEventCondition(ToLocation));

            ActionsOnPass.Add(new GenericEventAction(MoveFromSlotToGround));
        }

        public Location FromLocation { get; }

        public byte FromSlot { get; }

        public Location ToLocation { get; }

        public ITile ToTile { get; }

        public IItem Item { get; }

        public byte Count { get; }

        private void MoveFromSlotToGround()
        {
            if (Item == null || Requestor == null)
            {
                return;
            }


			// attempt to remove the item from the inventory
			var movingItem = Requestor.Inventory?.Remove(FromSlot, Count, out Boolean partialRemove);

			if (movingItem == null)
            {
                return;
            }

            // add the remaining item to the destination tile.
            IThing thing = movingItem;
            ToTile.AddThing(ref thing, movingItem.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, ToTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, ToTile.Location)), ToTile.Location);

            // and handle collision.
            if (!ToTile.HandlesCollision)
            {
                return;
            }

            foreach (var itemWithCollision in ToTile.ItemsWithCollision)
            {
                var collisionEvents = Game.Instance.EventsCatalog[ItemEventType.Collision].Cast<CollisionItemEvent>();

                var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, thing, Requestor as IPlayer) && e.CanBeExecuted);

                // Execute all actions.
                candidate?.Execute();
            }
        }
    }
}