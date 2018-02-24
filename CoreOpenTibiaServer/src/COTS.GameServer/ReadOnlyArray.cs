using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace COTS.GameServer {

    public sealed class ReadOnlyArray<T> : IReadOnlyList<T> {
        private readonly T[] _items;

        private ReadOnlyArray(T[] items) {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            this._items = items;
        }

        [JsonConstructor]
        private ReadOnlyArray(IEnumerable<T> items) {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _items = items.ToArray();
        }

        public T this[int index] => _items[index];

        public int Count => _items.Length;

        public IEnumerator<T> GetEnumerator() {
            return _items.GetGenericEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _items.GetEnumerator();
        }

        public static ReadOnlyArray<T> WrapCollection(T[] items) {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var readOnlyArray = new ReadOnlyArray<T>(items);
            return readOnlyArray;
        }
    }
}