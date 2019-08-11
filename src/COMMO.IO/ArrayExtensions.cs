using System;

namespace COMMO.IO
{
    public static class ArrayExtensions
    {
        public static T[] Combine<T>(this T[] first, T[] second)
        {
            var result = new T[first.Length + second.Length];

            Buffer.BlockCopy(first, 0, result, 0, first.Length);

            Buffer.BlockCopy(second, 0, result, first.Length, second.Length);

            return result;
        }

        public static T[] Combine<T>(this T[] first, T[] second, T[] third)
        {
            var result = new T[first.Length + second.Length + third.Length];

            Buffer.BlockCopy(first, 0, result, 0, first.Length);

            Buffer.BlockCopy(second, 0, result, first.Length, second.Length);

            Buffer.BlockCopy(third, 0, result, first.Length + second.Length, third.Length);
            
            return result;
        }

        public static T[] Combine<T>(this T[] first, T[] second, T[] third, T[] fourth)
        {
            var result = new T[first.Length + second.Length + third.Length + fourth.Length];

            Buffer.BlockCopy(first, 0, result, 0, first.Length);

            Buffer.BlockCopy(second, 0, result, first.Length, second.Length);

            Buffer.BlockCopy(third, 0, result, first.Length + second.Length, third.Length);

            Buffer.BlockCopy(fourth, 0, result, first.Length + second.Length + third.Length, fourth.Length);

            return result;
        }

        private static readonly Random _random = new Random();

        public static T[] Shuffle<T>(this T[] array)
        {
            int currentIndex = array.Length - 1;

            while (currentIndex > 0)
            {
                int newIndex = _random.Next(0, currentIndex + 1);

                if (currentIndex != newIndex)
                {
                    var temp = array[currentIndex]; array[currentIndex] = array[newIndex]; array[newIndex] = temp;
                }

                currentIndex--;
            }

            return array;
        }
    }
}