// <copyright file="LocationHasTileWithGroundEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace COMMO.Server.Movement.EventConditions
{
    using COMMO.Scheduling.Contracts;
    using COMMO.Server.Data.Models.Structs;

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
            Location = location;
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
            return Game.Instance.GetTileAt(Location)?.Ground != null;
        }
    }
}