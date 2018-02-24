using System;
using System.Diagnostics;

namespace COTS.GameServer {

    public sealed class Matrix<T> {

        /// <summary>
        /// Returns the height of this Matrix.
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// Returns the width of this Matrix.
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// Returns a reference to the underlaying array.
        /// Items are stored in a row major fashion.
        /// </summary>
        /// <remarks>
        /// I know this `exposure  implementation details can be considered a bad pratice,
        /// but for performance-critical sections this is a must.
        /// </remarks>
        public readonly T[] Items;

        /// <summary>
        /// Creates a new instance of Matrix with the provided paramters.
        /// </summary>
        public Matrix(int width, int height) {
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width) + " must be equal to or greater than zero.");
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height) + " must be equal to or greater than zero.");

            Width = width;
            Height = height;
            Items = new T[width * height];
        }

        /// <summary>
        /// Returns a string description of this instance.
        /// </summary>
        public override string ToString() {
            return $"Width:{Width},Height:{Height}.";
        }

        /// <summary>
        /// Returns the hash code of this instance.
        /// </summary>
        public override int GetHashCode() {
            return HashHelper.Start
                .CombineHashCode(Width)
                .CombineHashCode(Height);
        }

        /// <summary>
        /// Returns true if <paramref name="obj"/> is a <seealso cref="Matrix{T}"/> and
        /// it's <seealso cref="Width"/>, <seealso cref="Height"/> and every object in
        /// <seealso cref="Items"/> are equal.
        /// Returns false otherwise.
        /// </summary>
        public override bool Equals(object obj) {
            var other = obj as Matrix<T>;
            if (other == null)
                return false;

            if (Width != other.Width)
                return false;
            if (Height != other.Height)
                return false;

            var first = Items;
            var second = other.Items;
            for (int i = 0; i < first.Length; i++) {
                if (!first[i].Equals(second[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the element in the <paramref name="x"/>th column
        /// of the <paramref name="y"/>th row without performing boundary checks.
        /// </summary>
        public T GetWithoutBoundaryChecks(int x, int y) {
#if DEBUG
            Debug.Assert(x >= 0 && x < Width);
            Debug.Assert(y >= 0 && y < Height);
#endif

            return Items[(y * Width) + x];
        }

        /// <summary>
        /// Returns the element in the <paramref name="x"/>th column
        /// of the <paramref name="y"/>th row.
        /// </summary>
        public T Get(int x, int y) {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException(nameof(x) + $" must be between [0,{Width - 1}]");
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException(nameof(y) + $" must be between [0,{Height - 1}]");

            return Items[(y * Width) + x];
        }

        /// <summary>
        /// Sets the element in the <paramref name="x"/>th column
        /// of the <paramref name="y"/>th row.
        /// </summary>
        public void Set(int x, int y, T value) {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException(nameof(x) + $" must be between [0,{Width - 1}]");
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException(nameof(y) + $" must be between [0,{Height - 1}]");

            Items[(y * Width) + x] = value;
        }

        /// <summary>
        /// Sets the element in the <paramref name="x"/>th column
        /// of the <paramref name="y"/>th row without performing boundary checks.
        /// </summary>
        public void SetWithoutBoundaryChecks(int x, int y, T value) {
#if DEBUG
            Debug.Assert(x >= 0 && x < Width);
            Debug.Assert(y >= 0 && y < Height);
#endif

            Items[(y * Width) + x] = value;
        }
    }
}