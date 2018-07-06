// <copyright file="RequestorIsInRangeToMoveEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using COMMO.Scheduling.Contracts;
using COMMO.Server.Data.Models.Structs;

namespace COMMO.Server.Movement.EventConditions
{
    /// <summary>
    /// Class that represents an event condition that evaluates whether a requestor is in range to move from a location.
    /// </summary>
    internal class RequestorIsInRangeToMoveEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestorIsInRangeToMoveEventCondition"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the requestor creature.</param>
        /// <param name="movingFrom">The location from where the move is happening.</param>
        public RequestorIsInRangeToMoveEventCondition(uint requestorId, Location movingFrom)
        {
            RequestorId = requestorId;
            FromLocation = movingFrom;
        }

        /// <summary>
        /// Gets the id of the requesting creature.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets the location to check.
        /// </summary>
        public Location FromLocation { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "You are too far away.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (requestor == null)
            {
                // script called, probably
                return true;
            }

            return (requestor.Location - FromLocation).MaxValueIn2D <= 1;
        }
    }
}