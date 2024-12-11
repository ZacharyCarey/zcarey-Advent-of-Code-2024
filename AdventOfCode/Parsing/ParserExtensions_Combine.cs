using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Parsing
{
    public static partial class Parser
    {
        public static IEnumerable<T> Combine<T>(this IEnumerable<IEnumerable<T>> input)
        {
            foreach (IEnumerable<T> list in input)
            {
                foreach (T item in list)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> Combine<T, TInput>(this IEnumerable<TInput> input) where TInput : IEnumerable<T>
        {
            foreach (TInput list in input)
            {
                foreach (T item in list)
                {
                    yield return item;
                }
            }
        }
    }
}
