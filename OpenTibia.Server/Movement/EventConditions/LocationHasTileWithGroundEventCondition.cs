// <copyright file="LocationHasTileWithGroundEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.EventConditions
{
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Models.Structs;

    /// <summary>
    /// Class that represents an event condition that evaluates whether a location has a tile with ground on it.
    /// </summary>
    internal class LocationHasTileWithGroundEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationHasTileWithGroundEventCondition"/> class.
        /// </summary>
        /// <param name="location">The location to check.</param>
        public LocationHasTileWithGroundEventCondition(Location location)
        {
            this.Location = location;
        }

        /// <summary>
        /// Gets the location to check.
        /// </summary>
        public Location Location { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "There is not enough room.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            return Game.Instance.GetTileAt(this.Location)?.Ground != null;
        }
    }
}