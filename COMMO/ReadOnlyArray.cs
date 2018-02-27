using System;
using System.Collections;
using System.Collections.Generic;

namespace COMMO {

	public sealed class ReadOnlyArray<T> : IReadOnlyList<T> {
		private readonly T[] _items;

		private ReadOnlyArray(T[] items) {
			if (items == null)
				throw new ArgumentNullException(nameof(items));

			_items = items;
		}

		public T this[int index] => _items[index];

		public int Count => _items.Length;

		public IEnumerator<T> GetEnumerator() => _items.GetGenericEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

		public static ReadOnlyArray<T> WrapCollection(T[] items) {
			if (items == null)
				throw new ArgumentNullException(nameof(items));

			return new ReadOnlyArray<T>(items);
		}
	}
}