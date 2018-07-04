// <copyright file="CharacterListItem.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets
{
    using System.Net;
    using OpenTibia.Common.Helpers;

    public class CharacterListItem : ICharacterListItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterListItem"/> class.
        /// </summary>
        /// <param name="name">The character name to use.</param>
        /// <param name="ip">The public IP address for the world's connection.</param>
        /// <param name="port">The public port for the world's connection.</param>
        /// <param name="world">The world where this character lives.</param>
        public CharacterListItem(string name, IPAddress ip, ushort port, string world)
        {
            name.ThrowIfNullOrWhiteSpace();
            ip.ThrowIfNull();
            port.ThrowIfDefaultValue();
            world.ThrowIfNullOrWhiteSpace();

            this.Name = name;
            this.World = world;
            this.Ip = ip.GetAddressBytes();
            this.Port = port;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string World { get; }

        /// <inheritdoc/>
        public byte[] Ip { get; }

        /// <inheritdoc/>
        public ushort Port { get; }
    }
}