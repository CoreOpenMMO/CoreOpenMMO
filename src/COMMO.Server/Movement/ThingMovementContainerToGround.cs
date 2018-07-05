// <copyright file="ThingMovementContainerToGround.cs" company="2Dudes">
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
    internal class ThingMovementContainerToGround : MovementBase
    {
        public ThingMovementContainerToGround(uint creatureRequestingId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base(creatureRequestingId, EvaluationTime.OnExecute)
        {
            // intentionally left thing null check out. Handled by Perform().
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            if (requestor == null)
            {
                throw new ArgumentNullException(nameof(requestor));
            }

            Thing = thingMoving;
            Count = count;

            FromLocation = fromLocation;
            FromContainer = (requestor as IPlayer)?.GetContainer(FromLocation.Container);
            FromIndex = (byte)FromLocation.Z;

            ToLocation = toLocation;
            ToTile = Game.Instance.GetTileAt(ToLocation);

            Conditions.Add(new CanThrowBetweenEventCondition(RequestorId, requestor.Location, ToLocation));
            Conditions.Add(new GrabberHasContainerOpenEventCondition(RequestorId, FromContainer));
            Conditions.Add(new ContainerHasItemAndEnoughAmountEventCondition(Thing as IItem, FromContainer, FromIndex, Count));
            Conditions.Add(new LocationNotObstructedEventCondition(RequestorId, Thing, ToLocation));
            Conditions.Add(new LocationHasTileWithGroundEventCondition(ToLocation));

            ActionsOnPass.Add(new GenericEventAction(MoveContainerToGround));
        }

        public Location FromLocation { get; }

        public IContainer FromContainer { get; }

        public byte FromIndex { get; }

        public Location ToLocation { get; }

        public ITile ToTile { get; }

        public IThing Thing { get; }

        public byte Count { get; }

        private void MoveContainerToGround()
        {
            IItem extraItem;
            var itemToUpdate = Thing as IItem;

            if (FromContainer == null || ToTile == null || itemToUpdate == null)
            {
                return;
            }

            // attempt to remove from the source container
            if (!FromContainer.RemoveContent(itemToUpdate.Type.TypeId, FromIndex, Count, out extraItem))
            {
                return;
            }

            if (extraItem != null)
            {
                itemToUpdate = extraItem;
            }

            // add the remaining item to the destination tile.
            IThing thing = itemToUpdate;
            ToTile.AddThing(ref thing, itemToUpdate.Count);

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