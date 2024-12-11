using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Day_04
{
    internal class Program : ProgramStructure<CharMap>
    {

        Program() : base(input => input
            .GetLines()
            .Create<IEnumerable<string>, CharMap>()
        )
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(CharMap input)
        {
            long sum = 0;
            for(int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    sum += WordSearch(input, new Point(x, y), "XMAS");
                }
            }
            return sum;
        }

        protected override object SolvePart2(CharMap input)
        {
            long sum = 0;
            for (int y = 1; y < input.Height - 1; y++)
            {
                for (int x = 1; x < input.Width - 1; x++)
                {
                    if (XMas(input, new Point(x, y))) sum++;
                }
            }
            return sum;
        }

        private static bool XMas(CharMap map, Point loc)
        {
            if (map[loc] != 'A') return false;

            char TL = map[loc.X - 1, loc.Y - 1];
            char BR = map[loc.X + 1, loc.Y + 1];
            if (TL == 'M')
            {
                if (BR != 'S') return false;
            } else if (TL == 'S')
            {
                if (BR != 'M') return false;
            } else
            {
                return false;
            }

            char TR = map[loc.X + 1, loc.Y - 1];
            char BL = map[loc.X - 1, loc.Y + 1];
            if (TR == 'M') return BL == 'S';
            else if (TR == 'S') return BL == 'M';
            else return false;
        }

        private static long WordSearch(CharMap map, Point searchLoc, string word)
        {
            long sum = 0;
            for(int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 && y == 0) continue;
                    if (WordSearch(map, searchLoc, new Size(x, y), word))
                    {
                        sum++;
                    }
                }
            }
            return sum;
        }

        private static bool WordSearch(CharMap map, Point searchLoc, Size searchDir, string word)
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (searchLoc.X < 0 || searchLoc.X >= map.Width || searchLoc.Y < 0 || searchLoc.Y >= map.Height) return false;
                if (map[searchLoc] != word[i]) return false;
                searchLoc += searchDir;
            }
            return true;
        }
    }

    public class CharMap : IObjectParser<IEnumerable<string>, CharMap>
    {
        string[] Data;
        public int Width;
        public int Height;

        public CharMap(string[] data)
        {
            this.Data = data;
            this.Height = data.Length;
            this.Width = 0;
            foreach(var str in data)
            {
                if (str.Length > Width)
                {
                    Width = str.Length;
                }
            }
        }

        public char this[int x, int y]
        {
            get => Data[y][x];
        }

        public char this[Point p]
        {
            get => Data[p.Y][p.X];
        }

        public static CharMap Parse(IEnumerable<string> input)
        {
            return new CharMap(input.ToArray());
        }
    }
}
