using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Parsing
{

    public static partial class Parser
    {
        /// <summary>
        /// Returns the lines of strings that are separated by a single blank line (text blocks)
        /// </summary>
        public static IEnumerable<List<string>> GetBlocks(this IEnumerable<string> input)
        {
            List<string> group = new();
            foreach (string line in input)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    yield return group;
                    group = new();
                } else
                {
                    group.Add(line);
                }
            }

            if (group.Count > 0)
            {
                yield return group;
            }
        }

        public static IEnumerable<List<string>> GetBlocks(this string input)
        {
            return input.GetLines().GetBlocks();
        }
    }
}
