// <copyright file="ITile.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Data.Interfaces
{
    using System.Collections.Generic;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Models.Structs;

    public interface ITile
    {
        bool HandlesCollision { get; }

        IEnumerable<IItem> ItemsWithCollision { get; }

        bool HandlesSeparation { get; }

        IEnumerable<IItem> ItemsWithSeparation { get; }

        Location Location { get; }

        byte Flags { get; }

        bool IsHouse { get; }

        bool BlocksThrow { get; }

        bool BlocksPass { get; }

        bool BlocksLay { get; }

        IItem Ground { get; }

        IEnumerable<uint> CreatureIds { get; }

        IEnumerable<IItem> TopItems1 { get; }

        IEnumerable<IItem> TopItems2 { get; }

        IEnumerable<IItem> DownItems { get; }

        byte[] CachedDescription { get; }

        void AddThing(ref IThing thing, byte count = 1);

        void RemoveThing(ref IThing thing, byte count = 1);

        IThing GetThingAtStackPosition(byte stackPosition);

        byte GetStackPosition(IThing thing);

        void SetFlag(TileFlag flag);

        bool HasThing(IThing thing, byte count = 1);

        IItem BruteFindItemWithId(ushort typeId);

        IItem BruteRemoveItemWithId(ushort id);

        bool CanBeWalked(byte avoidDamageType = 0);
    }
}
