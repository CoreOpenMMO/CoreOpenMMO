// <copyright file="UnpassItemsInRangeEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.EventConditions
{
    using OpenTibia.Data.Contracts;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    /// <summary>
    /// Class that represents an event condition that evaluates whether an item with the unpass flag is being moved within range.
    /// </summary>
    internal class UnpassItemsInRangeEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnpassItemsInRangeEventCondition"/> class.
        /// </summary>
        /// <param name="moverId">The id of the creature requesting the move.</param>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="targetLoc">The location to which it's being moved.</param>
        public UnpassItemsInRangeEventCondition(uint moverId, IThing thingMoving, Location targetLoc)
        {
            this.MoverId = moverId;
            this.Thing = thingMoving;
            this.ToLocation = targetLoc;
        }

        /// <summary>
        /// Gets the target location.
        /// </summary>
        public Location ToLocation { get; }

        /// <summary>
        /// Gets the <see cref="IThing"/> being moved.
        /// </summary>
        public IThing Thing { get; }

        /// <summary>
        /// Gets the id of the mover.
        /// </summary>
        public uint MoverId { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "Sorry, not possible.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var mover = this.MoverId == 0 ? null : Game.Instance.GetCreatureWithId(this.MoverId);
            var item = this.Thing as IItem;

            if (item == null || mover == null || !item.Type.Flags.Contains(ItemFlag.Unpass))
            {
                // MoverId being null means this is probably a script's action.
                // Policy does not apply to this thing.
                return true;
            }

            var locDiff = mover.Location - this.ToLocation;

            return locDiff.Z == 0 && locDiff.MaxValueIn2D <= 2;
        }
    }
}