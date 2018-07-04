// <copyright file="TileContainsThingEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.EventConditions
{
    using System;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    /// <summary>
    /// Class that represents an event condition that evaluates whether a tile in a location contains the specified thing.
    /// </summary>
    internal class TileContainsThingEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileContainsThingEventCondition"/> class.
        /// </summary>
        /// <param name="thing">The thing to check for.</param>
        /// <param name="location">The location to check at.</param>
        /// <param name="count">Optional. The amount to check for. Default is 1.</param>
        public TileContainsThingEventCondition(IThing thing, Location location, byte count = 1)
        {
            if (count == 0 || count > 100)
            {
                throw new ArgumentException($"Invalid count {count}.", nameof(count));
            }

            this.Thing = thing;
            this.Count = count;
            this.Location = location;
        }

        /// <summary>
        /// Gets the location to check.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the <see cref="IThing"/> to check.
        /// </summary>
        public IThing Thing { get; }

        /// <summary>
        /// Gets the amount to check for.
        /// </summary>
        public byte Count { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "Sorry, not possible.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            if (this.Thing == null)
            {
                return false;
            }

            var sourceTile = Game.Instance.GetTileAt(this.Location);

            if (sourceTile == null || !sourceTile.HasThing(this.Thing))
            {
                // This tile no longer has the thing, or it's obstructed (i.e. someone placed something on top of it).
                return false;
            }

            return true;
        }
    }
}