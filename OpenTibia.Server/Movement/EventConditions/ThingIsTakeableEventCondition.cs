// <copyright file="ThingIsTakeableEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.EventConditions
{
    using OpenTibia.Data.Contracts;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Interfaces;

    /// <summary>
    /// Class that represents an event condition that evaluates if a thing is takeable.
    /// </summary>
    internal class ThingIsTakeableEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThingIsTakeableEventCondition"/> class.
        /// </summary>
        /// <param name="grabberId">The id of the grabber creature.</param>
        /// <param name="thingMoving">The thing being checked.</param>
        public ThingIsTakeableEventCondition(uint grabberId, IThing thingMoving)
        {
            this.GrabberId = grabberId;
            this.Thing = thingMoving;
        }

        /// <summary>
        /// Gets the <see cref="IThing"/> to check.
        /// </summary>
        public IThing Thing { get; }

        /// <summary>
        /// Gets the id of the grabber creature.
        /// </summary>
        public uint GrabberId { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "You may not move this object.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var grabber = this.GrabberId == 0 ? null : Game.Instance.GetCreatureWithId(this.GrabberId);

            if (grabber == null || this.Thing == null)
            {
                return false;
            }

            var item = this.Thing as IItem;

            // TODO: GrabberId access level?
            return item != null && item.Type.Flags.Contains(ItemFlag.Take);
        }
    }
}