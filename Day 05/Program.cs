using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day_05
{
    internal class Program : ProgramStructure<Day05Input>
    {

        Program() : base(input => new Day05Input(input))
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(Day05Input input)
        {
            return input.Updates
                .Where(update => IsUpdateOrdered(input, update))
                .Select(update => update[update.Count / 2])
                .Sum();
        }

        private static bool IsUpdateOrdered(Day05Input input, List<long> update)
        {
            HashSet<long> seenValues = new();
            foreach (long page in update)
            {
                foreach (var rule in input.Rules)
                {
                    if (rule.Item1 == page)
                    {
                        // Verify the second number hasnt been seen before
                        if (seenValues.Contains(rule.Item2))
                        {
                            return false;
                        }
                    }
                }
                seenValues.Add(page);
            }
            return true;
        }

        protected override object SolvePart2(Day05Input input)
        {
            return input.Updates
                .Where(update => !IsUpdateOrdered(input, update))
                .Select(update => {
                    update.Sort((x, y) => PageComparison(input, x, y));
                    return update[update.Count / 2];
                })
                .Sum();
        }

        private int PageComparison(Day05Input input, long x, long y)
        {
            // Lol feelzing lazy today
            foreach (var rule in input.Rules)
            {
                if (x == rule.Item1 && y == rule.Item2)
                {
                    // X should be before y
                    return -1;
                }
                else if (x == rule.Item2 && y == rule.Item1)
                {
                    // y should be before x
                    return 1;
                }
            }

            // Dont care about the order of these pages
            return 0;
        }
    }

    public class Day05Input
    {
        public List<Tuple<long, long>> Rules = new();
        public List<List<long>> Updates = new();

        public Day05Input(string input)
        {
            var blocks = input.GetBlocks().ToList();
            this.Rules = blocks[0]
                .Select(x => x.Split('|'))
                .Select(x => new Tuple<long, long>(long.Parse(x[0]), long.Parse(x[1])))
                .ToList();

            this.Updates = blocks[1]
                .Select(line =>
                    line.Split(',')
                    .Select(long.Parse)
                    .ToList())
                .ToList();
        }
    }
}
