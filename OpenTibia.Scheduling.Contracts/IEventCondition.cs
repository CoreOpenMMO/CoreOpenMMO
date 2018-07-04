// <copyright file="IEventFunction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Scheduling.Contracts
{
    /// <summary>
    /// Interface for event conditions.
    /// </summary>
    public interface IEventCondition
    {
        /// <summary>
        /// Gets the error message to use when the evaluation fails.
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// Evaluates the condition.
        /// </summary>
        /// <returns>True if the condition passes, false otherwise.</returns>
        bool Evaluate();
    }
}