using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day_03
{
    internal class Program : ProgramStructure<string>
    {
        private static readonly Regex MulRegex = new(@"mul\((\d+),(\d+)\)", RegexOptions.Compiled);
        private static readonly Regex CmdRegex = new(@"mul\((\d+),(\d+)\)|do\(\)|don't\(\)", RegexOptions.Compiled);

        Program() : base(x => x)
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(string input)
        {
            return MulRegex.Matches(input)
                .Select(match => (long.Parse(match.Groups[1].Value), long.Parse(match.Groups[2].Value)))
                .Select(numbers => numbers.Item1 * numbers.Item2)
                .Sum();
        }

        protected override object SolvePart2(string input)
        {
            long sum = 0;
            bool enabled = true;
            foreach (var match in CmdRegex.Matches(input).AsEnumerable())
            {
                if (match.Value.StartsWith("don't"))
                {
                    enabled = false;
                } else if (match.Value.StartsWith("do"))
                {
                    enabled = true;
                } else if (enabled)
                {
                    long num1 = long.Parse(match.Groups[1].Value);
                    long num2 = long.Parse(match.Groups[2].Value);
                    sum += num1 * num2;
                }
            }

            return sum;
        }

    }
}
