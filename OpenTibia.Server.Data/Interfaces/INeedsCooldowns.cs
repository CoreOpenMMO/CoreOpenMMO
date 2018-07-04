// <copyright file="INeedsCooldowns.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Data.Interfaces
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Data.Contracts;

    /// <summary>
    /// Interfaces any object that should keep cooldown state.
    /// </summary>
    public interface INeedsCooldowns
    {
        /// <summary>
        /// Gets the information about cooldowns for a creature where the key is a <see cref="CooldownType"/>, and the value is a <see cref="Tuple{T1, T2}"/>
        /// </summary>
        /// <remarks>The tuple elements are a <see cref="DateTime"/>, to store the time when the cooldown started, and a <see cref="TimeSpan"/> to denote how long it should last.</remarks>
        IDictionary<CooldownType, Tuple<DateTime, TimeSpan>> Cooldowns { get; }
    }
}