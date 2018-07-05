// <copyright file="ThingMovementContainerToContainer.cs" company="2Dudes">
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

    internal class ThingMovementContainerToContainer : MovementBase
    {
        public ThingMovementContainerToContainer(uint requestorId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
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

            Thing = thingMoving;
            Count = count;

            FromLocation = fromLocation;
            FromContainer = (Requestor as IPlayer)?.GetContainer(FromLocation.Container);
            FromIndex = (byte)FromLocation.Z;

            ToLocation = toLocation;
            ToContainer = (Requestor as IPlayer)?.GetContainer(ToLocation.Container);
            ToIndex = (byte)ToLocation.Z;

            if (FromContainer?.HolderId != ToContainer?.HolderId && ToContainer?.HolderId == RequestorId)
            {
                Conditions.Add(new GrabberHasEnoughCarryStrengthEventCondition(RequestorId, Thing));
            }

            Conditions.Add(new GrabberHasContainerOpenEventCondition(RequestorId, FromContainer));
            Conditions.Add(new ContainerHasItemAndEnoughAmountEventCondition(Thing as IItem, FromContainer, FromIndex, Count));
            Conditions.Add(new GrabberHasContainerOpenEventCondition(RequestorId, ToContainer));

            ActionsOnPass.Add(new GenericEventAction(MoveBetweenContainers));
        }

        public Location FromLocation { get; }

        public IContainer FromContainer { get; }

        public byte FromIndex { get; }

        public Location ToLocation { get; }

        public IContainer ToContainer { get; }

        public byte ToIndex { get; }

        public IThing Thing { get; }

        public byte Count { get; }

        private void MoveBetweenContainers()
        {
            IItem extraItem;
            var updatedItem = Thing as IItem;

            if (FromContainer == null || ToContainer == null || updatedItem == null || Requestor == null)
            {
                return;
            }

            // attempt to remove from the source container
            if (!FromContainer.RemoveContent(updatedItem.Type.TypeId, FromIndex, Count, out extraItem))
            {
                return;
            }

            if (extraItem != null)
            {
                updatedItem = extraItem;
            }

            // successfully removed thing from the source container
            // attempt to add to the dest container
            if (ToContainer.AddContent(updatedItem, ToIndex))
            {
                return;
            }

            // failed to add to the dest container (whole or partial)
            // attempt to add again to the source at any index.
            if (FromContainer.AddContent(updatedItem, 0xFF))
            {
                return;
            }

            // and we somehow failed to re-add it to the source container...
            // throw to the ground.
            IThing thing = updatedItem;
            Requestor.Tile.AddThing(ref thing, updatedItem.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, Requestor.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, Requestor.Location)), Requestor.Location);

            // and handle collision.
            if (!Requestor.Tile.HandlesCollision)
            {
                return;
            }

            foreach (var itemWithCollision in Requestor.Tile.ItemsWithCollision)
            {
                var collisionEvents = Game.Instance.EventsCatalog[ItemEventType.Collision].Cast<CollisionItemEvent>();

                var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, thing) && e.CanBeExecuted);

                // Execute all actions.
                candidate?.Execute();
            }
        }
    }
}