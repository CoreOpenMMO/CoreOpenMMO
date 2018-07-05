// <copyright file="ThingMovementOnMap.cs" company="2Dudes">
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

    internal class ThingMovementOnMap : MovementBase
    {
        public ThingMovementOnMap(uint creatureRequestingId, IThing thingMoving, Location fromLocation, byte fromStackPos, Location toLocation, byte count = 1, bool isTeleport = false)
            : base(creatureRequestingId, EvaluationTime.OnBoth)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(count));
            }

            FromLocation = fromLocation;
            FromStackPos = fromStackPos;
            FromTile = Game.Instance.GetTileAt(FromLocation);

            ToLocation = toLocation;
            ToTile = Game.Instance.GetTileAt(ToLocation);

            Thing = thingMoving;
            Count = count;
            IsTeleport = isTeleport;

            if (!isTeleport && Requestor != null)
            {
                Conditions.Add(new CanThrowBetweenEventCondition(RequestorId, FromLocation, ToLocation));
            }

            Conditions.Add(new RequestorIsInRangeToMoveEventCondition(RequestorId, FromLocation));
            Conditions.Add(new LocationNotObstructedEventCondition(RequestorId, Thing, ToLocation));
            Conditions.Add(new LocationHasTileWithGroundEventCondition(ToLocation));
            Conditions.Add(new UnpassItemsInRangeEventCondition(RequestorId, Thing, ToLocation));
            Conditions.Add(new LocationsMatchEventCondition(Thing?.Location ?? default(Location), FromLocation));
            Conditions.Add(new TileContainsThingEventCondition(Thing, FromLocation, Count));

            ActionsOnPass.Add(new GenericEventAction(MoveThing));
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
            if (FromTile == null || ToTile == null)
            {
                return;
            }

            var thing = Thing;

            FromTile.RemoveThing(ref thing, Count);

            ToTile.AddThing(ref thing, thing.Count);

            if (thing is ICreature)
            {
                Game.Instance.NotifySpectatingPlayers(
                    conn => new CreatureMovedNotification(
                        conn,
                        (thing as ICreature).CreatureId,
                        FromLocation,
                        FromStackPos,
                        ToLocation,
                        ToTile.GetStackPosition(Thing),
                        IsTeleport),
                    FromLocation,
                    ToLocation);
            }
            else
            {
                // TODO: see if we can save network bandwith here:
                // Game.Instance.NotifySpectatingPlayers(
                //        (conn) => new ItemMovedNotification(
                //            conn,
                //            (IItem)Thing,
                //            FromLocation,
                //            oldStackpos,
                //            ToLocation,
                //            destinationTile.GetStackPosition(Thing),
                //            false
                //        ),
                //        FromLocation,
                //        ToLocation
                //    );
                Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, FromLocation, Game.Instance.GetMapTileDescription(conn.PlayerId, FromLocation)), FromLocation);

                Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, ToLocation, Game.Instance.GetMapTileDescription(conn.PlayerId, ToLocation)), ToLocation);
            }

            if (FromTile.HandlesSeparation)
            {
                foreach (var itemWithSeparation in FromTile.ItemsWithSeparation)
                {
                    var separationEvents = Game.Instance.EventsCatalog[ItemEventType.Separation].Cast<SeparationItemEvent>();

                    var candidate = separationEvents.FirstOrDefault(e => e.ThingIdOfSeparation == itemWithSeparation.Type.TypeId && e.Setup(itemWithSeparation, thing, Requestor as IPlayer) && e.CanBeExecuted);

                    // Execute all actions.
                    candidate?.Execute();
                }
            }

            if (ToTile.HandlesCollision)
            {
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
}