// <copyright file="ICharacterListItem.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets
{
    /// <summary>
    /// Provides an interface for character list items.
    /// </summary>
    public interface ICharacterListItem
    {
        /// <summary>
        /// Gets the name of the character.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the name of the world where this character lives.
        /// </summary>
        string World { get; }

        /// <summary>
        /// Gets the IP address bytes that the client must use to connect if loging in with this character.
        /// </summary>
        byte[] Ip { get; }

        /// <summary>
        /// Gets the port that the client must use to connect if loging in with this character.
        /// </summary>
        ushort Port { get; }
    }
}