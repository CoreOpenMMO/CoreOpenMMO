// <copyright file="SlotHasContainerAndContainerHasEnoughCapacityEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Scheduling.Contracts;
using COMMO.Server.Data.Interfaces;

namespace COMMO.Server.Movement.EventConditions
{
    /// <summary>
    /// Class that represents an event condition that evaluates whether a slot has a container on it, and if that container has enough capacity on it.
    /// </summary>
    internal class SlotHasContainerAndContainerHasEnoughCapacityEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlotHasContainerAndContainerHasEnoughCapacityEventCondition"/> class.
        /// </summary>
        /// <param name="playerId">The id of the player to check.</param>
        /// <param name="itemInSlot">The item being checked.</param>
        public SlotHasContainerAndContainerHasEnoughCapacityEventCondition(uint playerId, IItem itemInSlot)
        {
            PlayerId = playerId;
            ItemInSlot = itemInSlot;
        }

        /// <summary>
        /// Gets the item being checked.
        /// </summary>
        public IItem ItemInSlot { get; }

        /// <summary>
        /// Gets the id of the player being checked.
        /// </summary>
        public uint PlayerId { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "The container is full.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var player = PlayerId == 0 ? null : Game.Instance.GetCreatureWithId(PlayerId);

            if (player == null)
            {
                return false;
            }

            if (ItemInSlot == null)
            {
                return true;
            }

            var itemAsContainer = ItemInSlot as IContainer;
            return !ItemInSlot.IsContainer || (itemAsContainer != null && itemAsContainer.Content.Count < itemAsContainer.Volume);
        }
    }
}