// <copyright file="ICharacterDeathInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets
{
    using System;

    /// <summary>
    /// Interface that represents a character's death information.
    /// </summary>
    public interface ICharacterDeathInfo
    {
        /// <summary>
        /// Gets the victim character player id.
        /// </summary>
        int VictimId { get; }

        /// <summary>
        /// Gets the victim character level.
        /// </summary>
        short VictimLevel { get; }

        /// <summary>
        /// Gets the killer player id, if available.
        /// </summary>
        int KillerId { get; }

        /// <summary>
        /// Gets the killer's name.
        /// </summary>
        string KillerName { get; }

        /// <summary>
        /// Gets a value indicating whether the killing was unjustified.
        /// </summary>
        bool Unjustified { get; }

        /// <summary>
        /// Gets the date and time of the death.
        /// </summary>
        DateTime Timestamp { get; }
    }
}
