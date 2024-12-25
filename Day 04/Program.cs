using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

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

    public class CharMap : IObjectParser<IEnumerable<string>, CharMap>, IEnumerable<IEnumerable<char>>, IObjectParser<string, CharMap>
    {
        char[,] Data;
        public int Width;
        public int Height;

        public CharMap(string[] data)
        {
            this.Height = data.Length;
            this.Width = data[0].Length;

            this.Data = new char[Width, Height];
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    this.Data[x, y] = data[y][x];
                }
            }
        }

        public char this[int x, int y]
        {
            get => Data[x, y];
            set => Data[x, y] = value;
        }

        public char this[Point p]
        {
            get => Data[p.X, p.Y];
            set => Data[p.X, p.Y] = value;
        }

        public IEnumerable<char> GetRow(int y)
        {
            return new Row(this, y);
        }

        public static CharMap Parse(IEnumerable<string> input)
        {
            return new CharMap(input.ToArray());
        }

        public IEnumerator<IEnumerable<char>> GetEnumerator()
        {
            return this.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerable<IEnumerable<char>> AsEnumerable()
        {
            for(int y = 0; y < this.Height; y++)
            {
                yield return new Row(this, y);
            }
        }

        private struct Row : IEnumerable<char>
        {
            private CharMap Map;
            private int RowNum;

            public Row(CharMap map, int row) {
                this.Map = map;
                this.RowNum = row;
            }

            public IEnumerator<char> GetEnumerator()
            {
                return this.AsEnumerable().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.AsEnumerable().GetEnumerator();
            }

            private IEnumerable<char> AsEnumerable()
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    yield return Map[x, RowNum];
                }
            }
        }

        public void Print()
        {
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    Console.Write(this.Data[x,y]);
                }
                Console.WriteLine();
            }
        }

        public static CharMap Parse(string input)
        {
            return CharMap.Parse(input.GetLines());
        }

        public IEnumerable<(Point Position, char Value)> AllPoints
        {
            get
            {
                for (int y = 0; y < this.Height; y++)
                {
                    for (int x = 0; x < this.Width; x++)
                    {
                        yield return (new Point(x, y), this[x, y]);
                    }
                }
            }
        }
    }

}
