// <copyright file="GrabberHasContainerOpenEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.EventConditions
{
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Interfaces;

    /// <summary>
    /// Class that represents an event condition that evaluates whether a grabber has the container open.
    /// </summary>
    internal class GrabberHasContainerOpenEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrabberHasContainerOpenEventCondition"/> class.
        /// </summary>
        /// <param name="grabberId">The id of the grabber creature.</param>
        /// <param name="destinationContainer">The container to check.</param>
        public GrabberHasContainerOpenEventCondition(uint grabberId, IContainer destinationContainer)
        {
            this.GrabberId = grabberId;
            this.TargetContainer = destinationContainer;
        }

        /// <summary>
        /// Gets the <see cref="IContainer"/> to check.
        /// </summary>
        public IContainer TargetContainer { get; }

        /// <summary>
        /// Gets the id of the creature to check.
        /// </summary>
        public uint GrabberId { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "Sorry, not possible.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var grabber = this.GrabberId == 0 ? null : Game.Instance.GetCreatureWithId(this.GrabberId);

            if (grabber == null || this.TargetContainer == null)
            {
                return false;
            }

            return !(grabber is IPlayer) || (grabber as IPlayer)?.GetContainerId(this.TargetContainer) >= 0;
        }
    }
}