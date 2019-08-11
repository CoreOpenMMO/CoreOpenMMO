// <copyright file="ThingMovementGroundToContainer.cs" company="2Dudes">
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
    internal class ThingMovementGroundToContainer : MovementBase
    {
        public ThingMovementGroundToContainer(uint requestorId, IThing thingMoving, Location fromLocation, byte fromStackPos, Location toLocation, byte count = 1)
            : base(requestorId, EvaluationTime.OnExecute)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            if (Requestor == null)
            {
                throw new ArgumentException("Invalid requestor id.", nameof(requestorId));
            }

            Thing = thingMoving;
            Count = count;

            FromLocation = fromLocation;
            FromStackPos = fromStackPos;
            FromTile = Game.Instance.GetTileAt(FromLocation);

            ToLocation = toLocation;
            ToContainer = (Requestor as IPlayer)?.GetContainer(toLocation.Container);
            ToIndex = (byte)ToLocation.Z;

            if (ToContainer != null && ToContainer.HolderId == RequestorId)
            {
                Conditions.Add(new GrabberHasEnoughCarryStrengthEventCondition(RequestorId, Thing));
            }

            Conditions.Add(new GrabberHasContainerOpenEventCondition(RequestorId, ToContainer));
            Conditions.Add(new ContainerHasEnoughCapacityEventCondition(ToContainer));
            Conditions.Add(new ThingIsTakeableEventCondition(RequestorId, Thing));
            Conditions.Add(new LocationsMatchEventCondition(Thing?.Location ?? default(Location), FromLocation));
            Conditions.Add(new TileContainsThingEventCondition(Thing, FromLocation, Count));

            ActionsOnPass.Add(new GenericEventAction(PickupToContainer));
        }

        public Location FromLocation { get; }

        public byte FromStackPos { get; }

        public ITile FromTile { get; }

        public Location ToLocation { get; }

        public IContainer ToContainer { get; }

        public byte ToIndex { get; }

        public IThing Thing { get; }

        public byte Count { get; }

        private void PickupToContainer()
        {
			if (FromTile == null || ToContainer == null || Thing == null || !(Thing is IItem thingAsItem)) {
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

            if (thing != Thing)
            {
                // item got split cause we removed less than the total amount.
                // update the thing we're adding to the container.
                thingAsItem = thing as IItem;
            }

            // attempt to add the item to the container.
            if (thingAsItem == null || ToContainer.AddContent(thingAsItem, ToIndex))
            {
                // and call any separation events.
                if (FromTile.HandlesSeparation) // TODO: what happens on separation of less than required quantity, etc?
                {
                    foreach (var itemWithSeparation in FromTile.ItemsWithSeparation)
                    {
                        var separationEvents = Game.Instance.EventsCatalog[ItemEventType.Separation].Cast<SeparationItemEvent>();

                        var candidate = separationEvents.FirstOrDefault(e => e.ThingIdOfSeparation == itemWithSeparation.Type.TypeId && e.Setup(itemWithSeparation, thing, Requestor as IPlayer) && e.CanBeExecuted);

                        // Execute all actions.
                        candidate?.Execute();
                    }
                }

                return;
            }

            // failed to add to the dest container (whole or partial)
            // add again to the source tile
            IThing itemAsThing = thingAsItem;
            FromTile.AddThing(ref itemAsThing, thingAsItem.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, FromTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, FromTile.Location)), FromTile.Location);

            // call any collision events again.
            if (!FromTile.HandlesCollision)
            {
                return;
            }

            foreach (var itemWithCollision in FromTile.ItemsWithCollision)
            {
                var collisionEvents = Game.Instance.EventsCatalog[ItemEventType.Collision].Cast<CollisionItemEvent>();

                var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, Thing) && e.CanBeExecuted);

                // Execute all actions.
                candidate?.Execute();
            }
        }
    }
}