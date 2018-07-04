// <copyright file="Outfit.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Data.Models.Structs
{
    public struct Outfit
    {
        public ushort LikeType { get; set; }

        public ushort Id { get; set; }

        public byte Head { get; set; }

        public byte Body { get; set; }

        public byte Legs { get; set; }

        public byte Feet { get; set; }
    }
}