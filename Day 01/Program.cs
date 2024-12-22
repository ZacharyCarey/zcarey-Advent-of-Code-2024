using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day_01
{
    internal class Program : ProgramStructure<List<IntArray>>
    {

        Program() : base(input => 
            input
            .GetLines()
            .Create<string, IntArray>()
            .ToList()
        )
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(List<IntArray> input)
        {
            List<int> list1 = input.Select(x => x.Left).ToList();
            list1.Sort();

            List<int> list2 = input.Select(x => x.Right).ToList();
            list2.Sort();

            return list1.Zip(list2).Select(x => Math.Abs(x.First - x.Second)).Sum();
        }

        protected override object SolvePart2(List<IntArray> input)
        {
            Dictionary<int, int> count = new();
            foreach(int right in input.Select(x => x.Right))
            {
                int current;
                if (!count.TryGetValue(right, out current))
                {
                    current = 0;
                }
                count[right] = current + 1;
            }

            return input
                .Select(x => x.Left)
                .Select(left =>
                {
                    int n;
                    if (!count.TryGetValue(left, out n))
                    {
                        n = 0;
                    }
                    return (Left: left, CountInRight: n);
                })
                .Select(x => x.Left * x.CountInRight)
                .Sum();
        }

    }

    public struct IntArray : IObjectParser<string, IntArray>, IEnumerable<int>
    {
        private static readonly Regex InputRegex = new(@"(\d+)", RegexOptions.Compiled);

        private int[] Values;
        public int Left => Values[0];
        public int Right => Values[1];
        public int Count => this.Values.Length;

        public int this[int index]
        {
            get => Values[index];
        }

        public IntArray(int[] values)
        {
            this.Values = values;
        }

        public static IntArray Parse(string line)
        {
            return new IntArray(
                InputRegex.Matches(line)
                .Select(match => int.Parse(match.Value))
                .ToArray()
            );
        }

        public static IntArray Parse(string line, bool compressed)
        {
            if (compressed) {
                return new IntArray(line.AsEnumerable().Select(x => int.Parse(x.ToString())).ToArray());
            } else
            {
                return Parse(line);
            }
        }

        public IEnumerator<int> GetEnumerator()
        {
            return this.Values.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Values.GetEnumerator();
        }
    }
}
