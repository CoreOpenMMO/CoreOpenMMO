// <copyright file="CanThrowBetweenEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.EventConditions
{
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Models.Structs;

    /// <summary>
    /// Class that represents a condition that evaluates whether a throw from A to B makes sense.
    /// </summary>
    internal class CanThrowBetweenEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CanThrowBetweenEventCondition"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the throw.</param>
        /// <param name="fromLocation">The start location.</param>
        /// <param name="toLocation">The end location.</param>
        /// <param name="checkLineOfSight">Whether or not to check the line of sight.</param>
        public CanThrowBetweenEventCondition(uint requestorId, Location fromLocation, Location toLocation, bool checkLineOfSight = true)
        {
            this.RequestorId = requestorId;
            this.FromLocation = fromLocation;
            this.ToLocation = toLocation;
            this.CheckLineOfSight = checkLineOfSight;
        }

        /// <summary>
        /// Gets the start location of the throw.
        /// </summary>
        public Location FromLocation { get; }

        /// <summary>
        /// Gets the end location of the throw.
        /// </summary>
        public Location ToLocation { get; }

        /// <summary>
        /// Gets a value indicating whether the line of sight should be checked.
        /// </summary>
        public bool CheckLineOfSight { get; }

        /// <summary>
        /// Gets the id of the creature requesting the throw.
        /// </summary>
        public uint RequestorId { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "You may not throw there.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);

            if (requestor == null)
            {
                // Empty requestorId means not a creature generated event... possibly a script.
                return true;
            }

            return Game.Instance.CanThrowBetween(this.FromLocation, this.ToLocation, this.CheckLineOfSight);
        }
    }
}