using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode.Parsing
{
    public interface IObjectParser<TInput, TSelf>
    {
        public static abstract TSelf Parse(TInput input);
    }

    public static partial class Parser
    {
        /// <summary>
        /// Creates a new object using a class that implements <see cref="IObjectParser{I}"/>. 
        /// Parsing is handled by the class, using the output from this parser as it's input.
        /// </summary>
        public static TOutput Create<TInput, TOutput>(this TInput input) where TOutput : IObjectParser<TInput, TOutput>
        {
            return TOutput.Parse(input);
        }

        public static IEnumerable<TOutput> Create<TInput, TOutput>(this IEnumerable<TInput> input) where TOutput : IObjectParser<TInput, TOutput>
        {
            return input.Select(TOutput.Parse);
        }

        public static TOutput Create<TOutput>(this string input) where TOutput : IObjectParser<string, TOutput>
        {
            return TOutput.Parse(input);
        }
    }

}
