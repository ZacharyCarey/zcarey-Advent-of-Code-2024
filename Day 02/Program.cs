using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using Day_01;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Day_02
{
    internal class Program : ProgramStructure<IntMap>
    {

        Program() : base(input =>
            input.GetLines().Create<IEnumerable<string>, IntMap>()
        )
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(IntMap input)
        {
            return input
                .Select(row => (Row: row, IsSafe: ValuesWithinRange(row, 0, 3) && (IsDecreasing(row) || IsIncreasing(row))))
                .Where(x => x.IsSafe)
                .Count();
        }

        protected override object SolvePart2(IntMap input)
        {
            // I'm feeling lazy today, so jsut brute force :)
            int safeCount = 0;
            foreach(var row in input)
            {
                if (IsSafe(row))
                {
                    safeCount++;
                } else
                {
                    for (int i = 0; i < row.Count; i++)
                    {
                        if (IsSafe(SkipAtIndex(row, i)))
                        {
                            safeCount++;
                            break;
                        }
                    }
                }
            }

            return safeCount;
        }

        private bool IsSafe(IEnumerable<int> values)
        {
            return ValuesWithinRange(values, 0, 3) && (IsDecreasing(values) || IsIncreasing(values));
        }

        private IEnumerable<int> SkipAtIndex(IntArray input, int skipIndex)
        {
            for(int i = 0; i < skipIndex; i++)
            {
                yield return input[i];
            }
            for(int i = skipIndex + 1; i < input.Count; i++)
            {
                yield return input[i];
            }
        }

        private bool ValuesWithinRange(IEnumerable<int> values, int minRange, int maxRange)
        {
            int last = values.First();
            foreach(int n in values.Skip(1))
            {
                int dif = Math.Abs(n - last);
                if (dif < minRange || dif > maxRange) return false;
                last = n;
            }
            return true;
        }

        private bool IsDecreasing(IEnumerable<int> values)
        {
            long last = long.MaxValue;
            foreach(long n in values)
            {
                if (n >= last) return false;
                last = n;
            }
            return true;
        }

        private bool IsIncreasing(IEnumerable<int> values)
        {
            long last = long.MinValue;
            foreach(long n in values)
            {
                if (n <= last) return false;
                last = n;
            }
            return true;
        }
    }

    public class IntMap : IObjectParser<IEnumerable<string>, IntMap>, IEnumerable<IntArray>
    {
        private IntArray[] Data;
        public int Width;
        public int Height => Data.Length;

        public IEnumerable<IntArray> Rows => Data;

        public IEnumerable<ColumnAccess> Columns {
            get
            {
                for (int i = 0; i < Width; i++)
                {
                    yield return new ColumnAccess(this, i);
                }
            }
        }

        public int this[int x, int y]
        {
            get => Data[y][x];
        }

        public int this[Point p]
        {
            get => Data[p.Y][p.X];
        }

        public IntMap(IntArray[] rows)
        {
            this.Data = rows;
            this.Width = 0;
            foreach(var row in rows)
            {
                if (row.Count > this.Width)
                {
                    this.Width = row.Count;
                }
            }
        }

        public IntArray Row(int row)
        {
            return this.Data[row];
        }

        public ColumnAccess Col(int column)
        {
            return new ColumnAccess(this, column);
        }

        public static IntMap Parse(IEnumerable<string> input)
        {
            return new IntMap(
                input.Create<string, IntArray>().ToArray()
            );
        }

        public static IntMap Parse(IEnumerable<string> input, bool compressed)
        {
            if (compressed)
            {
                return new IntMap(input.Select(x => IntArray.Parse(x, compressed)).ToArray());
            }
            else
            {
                return new IntMap(
                    input.Create<string, IntArray>().ToArray()
                );
            }
        }

        public IEnumerator<IntArray> GetEnumerator()
        {
            return this.Data.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Data.GetEnumerator();
        }
    }

    public struct ColumnAccess
    {
        private IntMap Map;
        private int Column;

        public ColumnAccess(IntMap map, int col)
        {
            this.Map = map;
            this.Column = col;
        }

        public int this[int row] => Map[Column, row];
    }
}
