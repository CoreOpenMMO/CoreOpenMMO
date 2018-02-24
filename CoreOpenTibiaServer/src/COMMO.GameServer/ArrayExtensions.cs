using System.Collections.Generic;

namespace COMMO.GameServer {

    public static class ArrayExtensions {

        public static IEnumerator<T> GetGenericEnumerator<T>(this T[] array) {
            return ((IEnumerable<T>)array).GetEnumerator();
        }
    }
}