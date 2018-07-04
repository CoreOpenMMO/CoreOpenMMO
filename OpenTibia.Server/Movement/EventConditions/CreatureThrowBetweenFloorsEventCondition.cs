// <copyright file="CreatureThrowBetweenFloorsEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.EventConditions
{
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    /// <summary>
    /// Class that represents a condition that evaluates whether a creature can throw a thing to a location on a different floor.
    /// </summary>
    internal class CreatureThrowBetweenFloorsEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureThrowBetweenFloorsEventCondition"/> class.
        /// </summary>
        /// <param name="creatureRequestingId">The id of the creature requesting the throw.</param>
        /// <param name="thingMoving">The thing that it is throwing.</param>
        /// <param name="toLocation">The location to where it is being thrown.</param>
        public CreatureThrowBetweenFloorsEventCondition(uint creatureRequestingId, IThing thingMoving, Location toLocation)
        {
            this.RequestorId = creatureRequestingId;
            this.Thing = thingMoving;
            this.ToLocation = toLocation;
        }

        /// <summary>
        /// Gets the location to where the thing is being thrown.
        /// </summary>
        public Location ToLocation { get; }

        /// <summary>
        /// Gets the id of the creature requesting the throw.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets the <see cref="IThing"/> being thrown.
        /// </summary>
        public IThing Thing { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "You may not throw there.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var thingAsCreature = this.Thing as ICreature;
            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);

            if (requestor == null || thingAsCreature == null)
            {
                // Not a creature requesting this one, possibly a script.
                // Or the thing moving is null, not this policy's job to restrict this...
                return true;
            }

            var locDiff = thingAsCreature.Location - this.ToLocation;

            return locDiff.Z == 0;
        }
    }
}