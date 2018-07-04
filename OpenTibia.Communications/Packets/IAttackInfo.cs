// <copyright file="IAttackInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets
{
    /// <summary>
    /// Interface that represents attack information.
    /// </summary>
    public interface IAttackInfo
    {
        /// <summary>
        /// Gets the id of the creature being attacked.
        /// </summary>
        uint CreatureId { get; }
    }
}