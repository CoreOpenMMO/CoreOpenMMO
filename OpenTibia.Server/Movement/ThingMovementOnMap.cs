// <copyright file="ThingMovementOnMap.cs" company="2Dudes">
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

    internal class ThingMovementOnMap : MovementBase
    {
        public ThingMovementOnMap(uint creatureRequestingId, IThing thingMoving, Location fromLocation, byte fromStackPos, Location toLocation, byte count = 1, bool isTeleport = false)
            : base(creatureRequestingId, EvaluationTime.OnBoth)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(count));
            }

            this.FromLocation = fromLocation;
            this.FromStackPos = fromStackPos;
            this.FromTile = Game.Instance.GetTileAt(this.FromLocation);

            this.ToLocation = toLocation;
            this.ToTile = Game.Instance.GetTileAt(this.ToLocation);

            this.Thing = thingMoving;
            this.Count = count;
            this.IsTeleport = isTeleport;

            if (!isTeleport && this.Requestor != null)
            {
                this.Conditions.Add(new CanThrowBetweenEventCondition(this.RequestorId, this.FromLocation, this.ToLocation));
            }

            this.Conditions.Add(new RequestorIsInRangeToMoveEventCondition(this.RequestorId, this.FromLocation));
            this.Conditions.Add(new LocationNotObstructedEventCondition(this.RequestorId, this.Thing, this.ToLocation));
            this.Conditions.Add(new LocationHasTileWithGroundEventCondition(this.ToLocation));
            this.Conditions.Add(new UnpassItemsInRangeEventCondition(this.RequestorId, this.Thing, this.ToLocation));
            this.Conditions.Add(new LocationsMatchEventCondition(this.Thing?.Location ?? default(Location), this.FromLocation));
            this.Conditions.Add(new TileContainsThingEventCondition(this.Thing, this.FromLocation, this.Count));

            this.ActionsOnPass.Add(new GenericEventAction(this.MoveThing));
        }

        public Location FromLocation { get; }

        public byte FromStackPos { get; }

        public ITile FromTile { get; }

        public Location ToLocation { get; }

        public ITile ToTile { get; }

        public IThing Thing { get; }

        public byte Count { get; }

        public bool IsTeleport { get; }

        private void MoveThing()
        {
            if (this.FromTile == null || this.ToTile == null)
            {
                return;
            }

            var thing = this.Thing;

            this.FromTile.RemoveThing(ref thing, this.Count);

            this.ToTile.AddThing(ref thing, thing.Count);

            if (thing is ICreature)
            {
                Game.Instance.NotifySpectatingPlayers(
                    conn => new CreatureMovedNotification(
                        conn,
                        (thing as ICreature).CreatureId,
                        this.FromLocation,
                        this.FromStackPos,
                        this.ToLocation,
                        this.ToTile.GetStackPosition(this.Thing),
                        this.IsTeleport),
                    this.FromLocation,
                    this.ToLocation);
            }
            else
            {
                // TODO: see if we can save network bandwith here:
                // Game.Instance.NotifySpectatingPlayers(
                //        (conn) => new ItemMovedNotification(
                //            conn,
                //            (IItem)this.Thing,
                //            this.FromLocation,
                //            oldStackpos,
                //            this.ToLocation,
                //            destinationTile.GetStackPosition(this.Thing),
                //            false
                //        ),
                //        this.FromLocation,
                //        this.ToLocation
                //    );
                Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, this.FromLocation, Game.Instance.GetMapTileDescription(conn.PlayerId, this.FromLocation)), this.FromLocation);

                Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, this.ToLocation, Game.Instance.GetMapTileDescription(conn.PlayerId, this.ToLocation)), this.ToLocation);
            }

            if (this.FromTile.HandlesSeparation)
            {
                foreach (var itemWithSeparation in this.FromTile.ItemsWithSeparation)
                {
                    var separationEvents = Game.Instance.EventsCatalog[ItemEventType.Separation].Cast<SeparationItemEvent>();

                    var candidate = separationEvents.FirstOrDefault(e => e.ThingIdOfSeparation == itemWithSeparation.Type.TypeId && e.Setup(itemWithSeparation, thing, this.Requestor as IPlayer) && e.CanBeExecuted);

                    // Execute all actions.
                    candidate?.Execute();
                }
            }

            if (this.ToTile.HandlesCollision)
            {
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
}