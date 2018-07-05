// <copyright file="CreatureMovementOnMap.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Movement
{
    using System;
    using COMMO.Data.Contracts;
    using COMMO.Server.Data.Interfaces;
    using COMMO.Server.Data.Models.Structs;
    using COMMO.Server.Movement.EventConditions;

    internal class CreatureMovementOnMap : ThingMovementOnMap
    {
        public Direction AttemptedDirection { get; }

        public CreatureMovementOnMap(uint requestorId, ICreature creatureMoving, Location fromLocation, Location toLocation, bool isTeleport = false, byte count = 1)
            : base(requestorId, creatureMoving, fromLocation, creatureMoving.GetStackPosition(), toLocation, count, isTeleport)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(count));
            }

            AttemptedDirection = fromLocation.DirectionTo(toLocation, true);

            // don't add any conditions if this wasn't a creature requesting.
            if (!IsTeleport && Requestor != null)
            {
                Conditions.Add(new LocationNotAviodEventCondition(RequestorId, Thing, ToLocation));
                Conditions.Add(new LocationsAreDistantByEventCondition(FromLocation, ToLocation));
                Conditions.Add(new CreatureThrowBetweenFloorsEventCondition(RequestorId, Thing, ToLocation));
            }

            ActionsOnPass.Add(new GenericEventAction(MoveCreature));
        }

        private void MoveCreature()
        {
            if (IsTeleport)
            {
                return;
            }

            // update both creature's to face the push direction... a *real* push!
            if (Requestor != Thing)
            {
                Requestor?.TurnToDirection(Requestor.Location.DirectionTo(Thing.Location));
            }

            ((Creature)Thing)?.TurnToDirection(AttemptedDirection);

            if (Requestor != null && Requestor == Thing)
            {
                Requestor.UpdateLastStepInfo(Requestor.NextStepId, wasDiagonal: AttemptedDirection > Direction.West);
            }
        }
    }
}