using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Day_25
{
    internal class Program : ProgramStructure<(Key[] Keys, Key[] Locks)>
    {

        Program() : base(Key.Parse)
        { }

        static void Main(string[] args)
        {
            new Program()
                //.Run(args);
                .Run(args, "Example.txt");
        }

        protected override object SolvePart1((Key[] Keys, Key[] Locks) input)
        {
            long validPairs = 0;
            foreach(var key in input.Keys)
            {
                foreach(var Lock in input.Locks)
                {
                    if (key.FitsInLock(Lock))
                    {
                        validPairs++;
                    }
                }
            }

            return validPairs;
        }

        protected override object SolvePart2((Key[] Keys, Key[] Locks) input)
        {
            return null;
        }

    }

    internal struct Key : IEnumerable<int>
    {
        public int[] Heights;

        public Key(int[] heights)
        {
            this.Heights = heights;
        }

        public static (Key[] Keys, Key[] Locks) Parse(string input)
        {
            List<Key> keys = new();
            List<Key> locks = new();

            var blocks = input.GetBlocks();
            foreach(var block in blocks)
            {
                bool isLock = (block[0].Where(c => c == '.').Count() == block[0].Length);
                int[] heights = new int[block[0].Length];

                foreach (string line in block)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (line[i] == '#')
                        {
                            heights[i]++;
                        }
                    }
                }

                // Subtract one from the heights
                for (int i = 0; i < heights.Length; i++)
                {
                    heights[i]--;
                }

                if (isLock)
                {
                    locks.Add(new Key(heights));
                } else
                {
                    keys.Add(new Key(heights));
                }
            }

            return (keys.ToArray(), locks.ToArray());
        }

        public IEnumerator<int> GetEnumerator()
        {
            return ((IEnumerable<int>)Heights).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Heights.GetEnumerator();
        }

        public bool FitsInLock(Key Lock)
        {
            for(int i = 0; i < Heights.Length; i++)
            {
                if (Heights[i] + Lock.Heights[i] > 5)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
