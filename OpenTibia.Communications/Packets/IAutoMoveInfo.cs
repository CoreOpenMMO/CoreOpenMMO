// <copyright file="IAutoMoveInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets
{
    using OpenTibia.Data.Contracts;

    /// <summary>
    /// Interface that represents the auto movement information.
    /// </summary>
    public interface IAutoMoveInfo
    {
        /// <summary>
        /// Gets the movement directions.
        /// </summary>
        Direction[] Directions { get; }
    }
}