// <copyright file="BinaryHeap.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Utilities
{
    using System;
    using System.Collections.Generic;

    public class BinaryHeap<T>
    {
        protected T[] Data;

        protected Comparison<T> Comparison;

        public BinaryHeap()
        {
            this.Constructor(4, null);
        }

        public BinaryHeap(Comparison<T> comparison)
        {
            this.Constructor(4, comparison);
        }

        public BinaryHeap(int capacity)
        {
            this.Constructor(capacity, null);
        }

        public BinaryHeap(int capacity, Comparison<T> comparison)
        {
            this.Constructor(capacity, comparison);
        }

        private void Constructor(int capacity, Comparison<T> comparison)
        {
            this.Data = new T[capacity];
            this.Comparison = comparison;
            if (this.Comparison == null)
            {
                this.Comparison = Comparer<T>.Default.Compare;
            }
        }

        public int Size { get; private set; }

        /// <summary>
        /// Add an item to the heap
        /// </summary>
        /// <param name="item"></param>
        public void Insert(T item)
        {
            if (this.Size == this.Data.Length)
            {
                this.Resize();
            }

            this.Data[this.Size] = item;
            this.HeapifyUp(this.Size);
            this.Size++;
        }

        /// <summary>
        /// Get the item of the root
        /// </summary>
        /// <returns></returns>
        public T Peak()
        {
            return this.Data[0];
        }

        /// <summary>
        /// Extract the item of the root
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            var item = this.Data[0];
            this.Size--;
            this.Data[0] = this.Data[this.Size];
            this.HeapifyDown(0);
            return item;
        }

        private void Resize()
        {
            var resizedData = new T[this.Data.Length * 2];
            Array.Copy(this.Data, 0, resizedData, 0, this.Data.Length);
            this.Data = resizedData;
        }

        private void HeapifyUp(int childIdx)
        {
            if (childIdx > 0)
            {
                var parentIdx = (childIdx - 1) / 2;
                if (this.Comparison.Invoke(this.Data[childIdx], this.Data[parentIdx]) > 0)
                {
                    // swap parent and child
                    var t = this.Data[parentIdx];
                    this.Data[parentIdx] = this.Data[childIdx];
                    this.Data[childIdx] = t;
                    this.HeapifyUp(parentIdx);
                }
            }
        }

        private void HeapifyDown(int parentIdx)
        {
            var leftChildIdx = (2 * parentIdx) + 1;
            var rightChildIdx = leftChildIdx + 1;
            var largestChildIdx = parentIdx;
            if (leftChildIdx < this.Size && this.Comparison.Invoke(this.Data[leftChildIdx], this.Data[largestChildIdx]) > 0)
            {
                largestChildIdx = leftChildIdx;
            }

            if (rightChildIdx < this.Size && this.Comparison.Invoke(this.Data[rightChildIdx], this.Data[largestChildIdx]) > 0)
            {
                largestChildIdx = rightChildIdx;
            }

            if (largestChildIdx != parentIdx)
            {
                var t = this.Data[parentIdx];
                this.Data[parentIdx] = this.Data[largestChildIdx];
                this.Data[largestChildIdx] = t;
                this.HeapifyDown(largestChildIdx);
            }
        }
    }
}
