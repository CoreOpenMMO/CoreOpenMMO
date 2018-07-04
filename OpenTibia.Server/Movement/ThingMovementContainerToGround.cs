// <copyright file="ThingMovementContainerToGround.cs" company="2Dudes">
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

    internal class ThingMovementContainerToGround : MovementBase
    {
        public ThingMovementContainerToGround(uint creatureRequestingId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base(creatureRequestingId, EvaluationTime.OnExecute)
        {
            // intentionally left thing null check out. Handled by Perform().
            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);

            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            if (requestor == null)
            {
                throw new ArgumentNullException(nameof(requestor));
            }

            this.Thing = thingMoving;
            this.Count = count;

            this.FromLocation = fromLocation;
            this.FromContainer = (requestor as IPlayer)?.GetContainer(this.FromLocation.Container);
            this.FromIndex = (byte)this.FromLocation.Z;

            this.ToLocation = toLocation;
            this.ToTile = Game.Instance.GetTileAt(this.ToLocation);

            this.Conditions.Add(new CanThrowBetweenEventCondition(this.RequestorId, requestor.Location, this.ToLocation));
            this.Conditions.Add(new GrabberHasContainerOpenEventCondition(this.RequestorId, this.FromContainer));
            this.Conditions.Add(new ContainerHasItemAndEnoughAmountEventCondition(this.Thing as IItem, this.FromContainer, this.FromIndex, this.Count));
            this.Conditions.Add(new LocationNotObstructedEventCondition(this.RequestorId, this.Thing, this.ToLocation));
            this.Conditions.Add(new LocationHasTileWithGroundEventCondition(this.ToLocation));

            this.ActionsOnPass.Add(new GenericEventAction(this.MoveContainerToGround));
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
            var itemToUpdate = this.Thing as IItem;

            if (this.FromContainer == null || this.ToTile == null || itemToUpdate == null)
            {
                return;
            }

            // attempt to remove from the source container
            if (!this.FromContainer.RemoveContent(itemToUpdate.Type.TypeId, this.FromIndex, this.Count, out extraItem))
            {
                return;
            }

            if (extraItem != null)
            {
                itemToUpdate = extraItem;
            }

            // add the remaining item to the destination tile.
            IThing thing = itemToUpdate;
            this.ToTile.AddThing(ref thing, itemToUpdate.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, this.ToTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, this.ToTile.Location)), this.ToTile.Location);

            // and handle collision.
            if (!this.ToTile.HandlesCollision)
            {
                return;
            }

            foreach (var itemWithCollision in this.ToTile.ItemsWithCollision)
            {
                var collisionEvents = Game.Instance.EventsCatalog[ItemEventType.Collision].Cast<CollisionItemEvent>();

                var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, thing, this.Requestor as IPlayer) && e.CanBeExecuted);

                // Execute all actions.
                candidate?.Execute();
            }
        }
    }
}