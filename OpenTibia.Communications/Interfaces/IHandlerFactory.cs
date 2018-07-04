// <copyright file="IHandlerFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Interfaces
{
    /// <summary>
    /// Interface that contains methods to create incoming packet handlers.
    /// </summary>
    public interface IHandlerFactory
    {
        /// <summary>
        /// Creates a new instance of a packet handler depending on the specified packet type.
        /// </summary>
        /// <param name="packeType">The packet type.</param>
        /// <returns>A new instance of an <see cref="IIncomingPacketHandler"/> implementaion.</returns>
        IIncomingPacketHandler CreateIncommingForType(byte packeType);
    }
}