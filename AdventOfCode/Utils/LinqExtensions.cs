using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Utils
{
    public static class LinqExtensions
    {

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T newElement)
        {
            if (source == null) throw new ArgumentNullException(nameof(source), "Enumberable was null.");
            foreach (T element in source) yield return element;
            yield return newElement;
        }

        public static IEnumerable<(int x, int y)> Indices<T>(this T[,] source)
        {
            for (int y = 0; y < source.GetLength(1); y++)
            {
                for (int x = 0; x < source.GetLength(0); x++)
                {
                    yield return (x, y);
                }
            }
        }

        public static IEnumerable<T> Values<T>(this T[,] source)
        {
            for (int y = 0; y < source.GetLength(1); y++)
            {
                for (int x = 0; x < source.GetLength(0); x++)
                {
                    yield return source[x, y];
                }
            }
        }

        public static IEnumerable<(int Index, T Element)> WithIndex<T>(this IEnumerable<T> source)
        {
            int index = 0;
            foreach (T element in source)
            {
                yield return (index++, element);
            }
        }

        public static IEnumerable<T> AsEnumerable<T>(this T[,] array)
        {
            for(int y = 0; y < array.GetLength(1); y++)
            {
                for (int x = 0; x < array.GetLength(0); x++)
                {
                    yield return array[x, y];
                }
            }
        }
    }
}
