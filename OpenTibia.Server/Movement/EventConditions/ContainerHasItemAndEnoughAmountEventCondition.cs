// <copyright file="ContainerHasItemAndEnoughAmountEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.EventConditions
{
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Interfaces;

    /// <summary>
    /// Class that represents a condition which evaluates if a container has at least X amount of a given item.
    /// </summary>
    internal class ContainerHasItemAndEnoughAmountEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerHasItemAndEnoughAmountEventCondition"/> class.
        /// </summary>
        /// <param name="itemToCheck">The item to check.</param>
        /// <param name="fromContainer">The container to check.</param>
        /// <param name="indexToCheck">The index within the container to check.</param>
        /// <param name="countToCheck">The item's count.</param>
        public ContainerHasItemAndEnoughAmountEventCondition(IItem itemToCheck, IContainer fromContainer, byte indexToCheck, byte countToCheck)
        {
            this.ItemToCheck = itemToCheck;
            this.FromContainer = fromContainer;
            this.FromIndex = indexToCheck;
            this.Count = countToCheck;
        }

        /// <summary>
        /// Gets the <see cref="IItem"/> to check.
        /// </summary>
        public IItem ItemToCheck { get; }

        /// <summary>
        /// Gets the <see cref="IContainer"/> to check.
        /// </summary>
        public IContainer FromContainer { get; }

        /// <summary>
        /// Gets the index within the container to check.
        /// </summary>
        public byte FromIndex { get; }

        /// <summary>
        /// Gets the count of the item to check for.
        /// </summary>
        public byte Count { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "There is not enough quantity.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            if (this.ItemToCheck == null || this.FromContainer == null)
            {
                return false;
            }

            return this.FromContainer.CountContentAmountAt(this.FromIndex, this.ItemToCheck.Type.TypeId) >= this.Count;
        }
    }
}