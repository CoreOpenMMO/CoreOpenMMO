// <copyright file="CreatureMovementOnMap.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement
{
    using System;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Movement.EventConditions;

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

            this.AttemptedDirection = fromLocation.DirectionTo(toLocation, true);

            // don't add any conditions if this wasn't a creature requesting.
            if (!this.IsTeleport && this.Requestor != null)
            {
                this.Conditions.Add(new LocationNotAviodEventCondition(this.RequestorId, this.Thing, this.ToLocation));
                this.Conditions.Add(new LocationsAreDistantByEventCondition(this.FromLocation, this.ToLocation));
                this.Conditions.Add(new CreatureThrowBetweenFloorsEventCondition(this.RequestorId, this.Thing, this.ToLocation));
            }

            this.ActionsOnPass.Add(new GenericEventAction(this.MoveCreature));
        }

        private void MoveCreature()
        {
            if (this.IsTeleport)
            {
                return;
            }

            // update both creature's to face the push direction... a *real* push!
            if (this.Requestor != this.Thing)
            {
                this.Requestor?.TurnToDirection(this.Requestor.Location.DirectionTo(this.Thing.Location));
            }

            ((Creature)this.Thing)?.TurnToDirection(this.AttemptedDirection);

            if (this.Requestor != null && this.Requestor == this.Thing)
            {
                this.Requestor.UpdateLastStepInfo(this.Requestor.NextStepId, wasDiagonal: this.AttemptedDirection > Direction.West);
            }
        }
    }
}