// <copyright file="DuplicateComparer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Utilities
{
    using System.Collections.Generic;

    /// <summary>
    /// System.Collections.Generic.SortedList by default does not allow duplicate items.
    /// Since items are keyed by TotalCost there can be duplicate entries per key.
    /// </summary>
    internal class DuplicateComparer : IComparer<int>
    {
        /// <summary>
        /// Compares an int to another.
        /// </summary>
        /// <param name="x">The first integer to compare.</param>
        /// <param name="y">The second integer to compare.c</param>
        /// <returns>-1 if first is less than or equal to second, 1 otherwise.</returns>
        public int Compare(int x, int y)
        {
            return (x <= y) ? -1 : 1;
        }
    }
}
