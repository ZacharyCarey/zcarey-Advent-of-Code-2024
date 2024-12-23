using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using Day_01;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace Day_11
{
    internal class Program : ProgramStructure<IntArray>
    {

        Program() : base(IntArray.Parse)
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(IntArray input)
        {
            LinkedList<string> rocks = new(input.Select(x => x.ToString()));
            //PrintRocks(rocks, 25);

            for(int blink = 0; blink < 25; blink++)
            {
                Simulate(rocks);
                //PrintRocks(rocks, 25);
            }
            return rocks.Count;
        }

        private static void PrintRocks(LinkedList<string> rocks, int limit)
        {
            IEnumerable<string> str = rocks;
            if (rocks.Count > limit) {
                str = rocks.Take(limit);
            }
            Console.WriteLine(string.Join(' ', str));
        }

        private static void Simulate(LinkedList<string> rocks)
        {
            for(var itr = rocks.First; itr != null; itr = itr.Next)
            {
                if (itr.Value == "0")
                {
                    itr.Value = "1";
                } else if (itr.Value.Length % 2 == 0)
                {
                    var newNode = rocks.AddBefore(itr, itr.Value[..(itr.Value.Length / 2)].TrimStart('0'));
                    if (newNode.Value.Length == 0) newNode.Value = "0";

                    itr.Value = itr.Value[(itr.Value.Length / 2)..].TrimStart('0');
                    if (itr.Value.Length == 0) itr.Value = "0";
                } else
                {
                    itr.Value = (long.Parse(itr.Value) * 2024).ToString();
                }
            }
        }

        protected override object SolvePart2(IntArray input)
        {
            // https://en.wikipedia.org/wiki/Memoization
            Dictionary<int, Dictionary<long, long>> numberOfSpawnedRocks = new();
            return input.Select(x => Simulate(x, 75, numberOfSpawnedRocks)).Sum();
        }

        // Calculates the number of total rocks given an input rock and blinks
        private static long Simulate(long rock, int blinks, Dictionary<int, Dictionary<long, long>> mem)
        {
            if (blinks == 0) return 1;

            Dictionary<long, long> blinkMem;
            if (!mem.TryGetValue(blinks, out blinkMem))
            {
                blinkMem = new Dictionary<long, long>();
                mem[blinks] = blinkMem;
            }

            long result;
            if (!blinkMem.TryGetValue(rock, out result))
            {
                // Have to simulate this value
                if (rock == 0)
                {
                    result = Simulate(1, blinks - 1, mem);
                }
                else if (Math.Floor(Math.Log10(rock) + 1) % 2 == 0)
                {
                    string str = rock.ToString();
                    result = Simulate(long.Parse(str[..(str.Length / 2)]), blinks - 1, mem)
                           + Simulate(long.Parse(str[(str.Length / 2)..]), blinks - 1, mem);
                }
                else
                {
                    result = Simulate(rock * 2024, blinks - 1, mem);
                }
                blinkMem[rock] = result;
            }

            return result;
        }
    }
}
