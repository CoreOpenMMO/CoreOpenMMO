namespace COMMO.Utilities {
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// This class is a wrapper for arrays, providing only read methods.
	/// </summary>
	/// <remarks>
	/// Since this is just a view of the actual array, if the underlaying array is mutated so are the values
	/// returned by the methods of this class.
	/// </remarks>
	public sealed class ReadOnlyArray<T> : IReadOnlyList<T> {
		private readonly T[] _items;

		private ReadOnlyArray(T[] items) {
			_items = items ?? throw new ArgumentNullException(nameof(items));
		}

		/// <summary>
		/// Returns the element at <paramref name="index"/>-th position.
		/// </summary>
		public T this[int index] => _items[index];

		/// <summary>
		/// Returns the number of elements the array can hold.
		/// </summary>
		public int Count => _items.Length;

		/// <summary>
		/// Returns a generic enumerator for the array.
		/// </summary>
		public IEnumerator<T> GetEnumerator() => _items.GetGenericEnumerator();

		/// <summary>
		/// Returns a non-generic enumerator for the array.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();


		/// <summary>
		/// Creates a new instance of <see cref="ReadOnlyArray{T}"/> to wrap the provided array.
		/// </summary>
		/// Since this is just a view of the actual array, if the underlaying array is mutated so are the values
		/// returned by the methods of this class.
		/// </remarks>
		public static ReadOnlyArray<T> WrapCollection(T[] items) {
			if (items == null)
				throw new ArgumentNullException(nameof(items));

			return new ReadOnlyArray<T>(items);
		}
	}
}
