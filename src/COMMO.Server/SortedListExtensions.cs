// <copyright file="SortedListExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

/*
astar-1.0.cs may be freely distributed under the MIT license.

Copyright (c) 2013 Josh Baldwin https://github.com/jbaldwin/astar.cs

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

namespace COMMO.Server {
	using COMMO.Utilities;
	using System.Collections.Generic;

	/// <summary>
	/// Extension methods to make the System.Collections.Generic.SortedList easier to use.
	/// </summary>
	public static class SortedListExtensions {
		/// <summary>
		/// Checks if the SortedList is empty.
		/// </summary>
		/// <typeparam name="TKey">The </typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="sortedList">SortedList to check if it is empty.</param>
		/// <returns>True if sortedList is empty, false if it still has elements.</returns>
		internal static bool IsEmpty<TKey, TValue>(this SortedList<TKey, TValue> sortedList) {
			return sortedList.Count == 0;
		}

		/// <summary>
		/// Adds a INode to the SortedList.
		/// </summary>
		/// <param name="sortedList">SortedList to add the node to.</param>
		/// <param name="node">Node to add to the sortedList.</param>
		internal static void Add(this SortedList<int, INode> sortedList, INode node) {
			sortedList.Add(node.TotalCost, node);
		}

		/// <summary>
		/// Removes the node from the sorted list with the smallest TotalCost and returns that node.
		/// </summary>
		/// <param name="sortedList">SortedList to remove and return the smallest TotalCost node.</param>
		/// <returns>Node with the smallest TotalCost.</returns>
		internal static INode Pop(this SortedList<int, INode> sortedList) {
			var top = sortedList.Values[0];
			sortedList.RemoveAt(0);
			return top;
		}
	}
}
