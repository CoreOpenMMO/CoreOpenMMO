// <copyright file="EvaluationTime.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Scheduling.Contracts
{
    /// <summary>
    /// Represents options for when to evaluate the conditionals defined on an <see cref="IEvent"/>.
    /// </summary>
    public enum EvaluationTime
    {
        /// <summary>
        /// Evaluation occurs and schedule time.
        /// </summary>
        OnSchedule,

        /// <summary>
        /// Evaluation occurs right before execution time.
        /// </summary>
        OnExecute,

        /// <summary>
        /// Evaluation occurs on both schedule and execution time.
        /// </summary>
        OnBoth
    }
}