// <copyright file="IInventory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Data.Interfaces
{
    public interface IInventory
    {
        ICreature Owner { get; }

        byte TotalAttack { get; }

        byte TotalDefense { get; }

        byte TotalArmor { get; }

        byte AttackRange { get; }

        IItem this[byte idx] { get; }

        bool Add(IItem item, out IItem extraItem, byte positionByte = 0xFF, byte count = 1, ushort lossProbability = 300);

        IItem Remove(byte positionByte, byte count, out bool wasPartial);

        IItem Remove(ushort itemId, byte count, out bool wasPartial);
    }
}
