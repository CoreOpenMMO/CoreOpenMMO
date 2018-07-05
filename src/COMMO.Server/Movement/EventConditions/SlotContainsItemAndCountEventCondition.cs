// <copyright file="SlotContainsItemAndCountEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Scheduling.Contracts;
using COMMO.Server.Data.Interfaces;
using COMMO.Server.Data.Models.Structs;

namespace COMMO.Server.Movement.EventConditions
{
    /// <summary>
    /// Class that represents an event condition that evaluates whether a slot contains an item and enough amount of it.
    /// </summary>
    internal class SlotContainsItemAndCountEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlotContainsItemAndCountEventCondition"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the requesting creature.</param>
        /// <param name="movingItem">The item to check.</param>
        /// <param name="slot">The slot to check.</param>
        /// <param name="count">The amount to check for.</param>
        public SlotContainsItemAndCountEventCondition(uint requestorId, IItem movingItem, byte slot, byte count = 1)
        {
            RequestorId = requestorId;
            ItemMoving = movingItem;
            Slot = slot;
            Count = count;
        }

        /// <summary>
        /// Gets the id of the requesting creature.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets the <see cref="IItem"/> being checked.
        /// </summary>
        public IItem ItemMoving { get; }

        /// <summary>
        /// Gets the slot being checked.
        /// </summary>
        public byte Slot { get; }

        /// <summary>
        /// Gets the amount expected.
        /// </summary>
        public byte Count { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "You are too far away.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (requestor == null || ItemMoving == null)
            {
                return false;
            }

            var itemAtSlot = requestor.Inventory?[Slot];

            return itemAtSlot != null && ItemMoving.Type.TypeId == itemAtSlot.Type.TypeId && itemAtSlot.Count >= Count;
        }
    }
}