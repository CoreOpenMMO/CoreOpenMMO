// <copyright file="LocationsAreDistantByEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.EventConditions
{
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Models.Structs;

    /// <summary>
    /// Class that represents an event condition that evaluates whether two locations are distant by at least a value, and if they should be on the same floor.
    /// </summary>
    internal class LocationsAreDistantByEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationsAreDistantByEventCondition"/> class.
        /// </summary>
        /// <param name="locationOne">The first location.</param>
        /// <param name="locationTwo">The second location.</param>
        /// <param name="distance">Optional. The minimum distance that the loations must be distant by. Default is 1.</param>
        /// <param name="sameFloorOnly">Optional. Whether or not the locations must be on the same floor. Default is false.</param>
        public LocationsAreDistantByEventCondition(Location locationOne, Location locationTwo, byte distance = 1, bool sameFloorOnly = false)
        {
            this.FirstLocation = locationOne;
            this.SecondLocation = locationTwo;
            this.Distance = distance;
            this.SameFloorOnly = sameFloorOnly;
        }

        /// <summary>
        /// Gets the first location.
        /// </summary>
        public Location FirstLocation { get; }

        /// <summary>
        /// Gets the second location.
        /// </summary>
        public Location SecondLocation { get; }

        /// <summary>
        /// Gets the distance.
        /// </summary>
        public byte Distance { get; }

        /// <summary>
        /// Gets a value indicating whether locations on the same floor only are allowed.
        /// </summary>
        public bool SameFloorOnly { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "The destination is too far away.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var locationDiff = (this.FirstLocation - this.SecondLocation).MaxValueIn2D;
            var sameFloor = this.FirstLocation.Z == this.SecondLocation.Z;

            if (locationDiff <= this.Distance && (!this.SameFloorOnly || sameFloor))
            {
                // The thing is no longer in this position.
                return true;
            }

            return false;
        }
    }
}