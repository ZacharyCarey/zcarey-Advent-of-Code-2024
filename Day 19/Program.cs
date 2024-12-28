using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day_19
{
    internal class Program : ProgramStructure<Day19Input>
    {

        Program() : base(Day19Input.Parse)
        { }

        static void Main(string[] args)
        {
            new Program().Run(args);
        }

        protected override object SolvePart1(Day19Input input)
        {
            patterns.Clear();
            return input.Designs.Where(x => DesignPossible(input.Towels, x) > 0).Count();
        }

        Dictionary<string, long> patterns = new();

        private long DesignPossible(string[] towels, string design)
        {
            return DesignPossible(towels, design, 0);
        }

        private long DesignPossible(string[] towels, string design, int current)
        {
            long count;
            string remaining = design[current..];
            if (patterns.TryGetValue(remaining, out count)) return count;

            count = 0;
            for (int i = 0; i < towels.Length; i++)
            {
                if (current + towels[i].Length > design.Length) continue;
                bool match = true;
                for (int j = 0; j < towels[i].Length; j++)
                {
                    if (towels[i][j] != design[current + j])
                    {
                        match = false;
                        break;
                    }
                }
                if (!match) continue;

                if (current + towels[i].Length == design.Length) count++;
                else
                {
                    // Length is less, we need to look for more towels
                    count += DesignPossible(towels, design, current + towels[i].Length);
                }
            }

            patterns[remaining] = count;
            return count;
        }

        protected override object SolvePart2(Day19Input input)
        {
            patterns.Clear();
            return input.Designs.Select(x => DesignPossible(input.Towels, x)).Sum();
        }

    }

    internal class Day19Input
    {
        public string[] Towels;
        public string[] Designs;

        public Day19Input(string input)
        {
            List<string>[] blocks = input.GetBlocks().ToArray();
            Towels = blocks[0][0].Split(", ");
            Designs = blocks[1].ToArray();
        }

        public static Day19Input Parse(string input)
        {
            return new Day19Input(input);
        }
    }
}
