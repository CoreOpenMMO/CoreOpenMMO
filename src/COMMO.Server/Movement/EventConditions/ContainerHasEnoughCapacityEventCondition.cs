// <copyright file="ContainerHasEnoughCapacityEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Scheduling.Contracts;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Server.Movement.EventConditions
{
    /// <summary>
    /// Class that represents a condition that evaluates whether a container has enough capacity.
    /// </summary>
    internal class ContainerHasEnoughCapacityEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerHasEnoughCapacityEventCondition"/> class.
        /// </summary>
        /// <param name="destinationContainer">The container to evaluate.</param>
        public ContainerHasEnoughCapacityEventCondition(IContainer destinationContainer)
        {
            TargetContainer = destinationContainer;
        }

        /// <summary>
        /// Gets the <see cref="IContainer"/> to evaluate.
        /// </summary>
        public IContainer TargetContainer { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "There is not enough room.";

		/// <inheritdoc/>
		public bool Evaluate() => TargetContainer != null && TargetContainer?.Volume - TargetContainer.Content.Count > 0;
	}
}