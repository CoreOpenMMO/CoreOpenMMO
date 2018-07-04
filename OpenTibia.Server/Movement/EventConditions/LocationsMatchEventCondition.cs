// <copyright file="LocationsMatchEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.EventConditions
{
    using OpenTibia.Server.Data.Models.Structs;

    /// <summary>
    /// Class that represents an event condition that evaluates whether two locations match.
    /// </summary>
    internal class LocationsMatchEventCondition : LocationsAreDistantByEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationsMatchEventCondition"/> class.
        /// </summary>
        /// <param name="locationOne">The first location.</param>
        /// <param name="locationTwo">The second location.</param>
        public LocationsMatchEventCondition(Location locationOne, Location locationTwo)
            : base(locationOne, locationTwo, 0, true)
        {
            // Nothing else...
        }
    }
}