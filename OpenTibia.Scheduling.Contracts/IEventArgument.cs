// <copyright file="IEventArgument.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Scheduling.Contracts
{
    using System;

    /// <summary>
    /// Interface for event arguments.
    /// </summary>
    public interface IEventArgument
    {
        /// <summary>
        /// Gets the argument's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the argument's type.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets the argument's value.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Gets a value indicating whether the argument is an array type.
        /// </summary>
        bool IsArray { get; }
    }
}