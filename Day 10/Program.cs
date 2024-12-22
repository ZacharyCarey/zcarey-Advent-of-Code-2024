using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using Day_02;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Day_10
{
    internal class Program : ProgramStructure<IntMap>
    {
        private static Size Up = new(0, -1);
        private static Size Right = new (1, 0);
        private static Size Down = new(0, 1);
        private static Size Left = new(-1, 0);

        Program() : base(x => IntMap.Parse(x.GetLines(), true))
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(IntMap input)
        {
            long score = 0;
            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    if (input[x, y] == 0)
                    {
                        score += GetTrailheadScore(input, new Point(x, y));
                    }
                }
            }

            return score;
        }

        private static int GetTrailheadScore(IntMap map, Point p)
        {
            HashSet<Point> summits = new HashSet<Point>();
            Trace(map, p, 0, summits);
            return summits.Count;
        }

        private static void Trace(IntMap map, Point p, int level, HashSet<Point> summits)
        {
            if (p.X < 0 || p.Y < 0 || p.X >= map.Width || p.Y >= map.Height) return;
            int here = map[p];
            if (here != level) return;
            if (here == 9)
            {
                summits.Add(p);
                return;
            }

            Trace(map, p + Up, level + 1, summits);
            Trace(map, p + Right, level + 1, summits);
            Trace(map, p + Down, level + 1, summits);
            Trace(map, p + Left, level + 1, summits);
        }

        protected override object SolvePart2(IntMap input)
        {
            long rating = 0;
            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    if (input[x, y] == 0)
                    {
                        rating += GetTrailheadInfo(input, new Point(x, y)).Rating;
                    }
                }
            }

            return rating;
        }

        private static (int Score, int Rating) GetTrailheadInfo(IntMap map, Point p)
        {
            HashSet<Point> summits = new HashSet<Point>();
            int rating = TraceRating(map, p, 0, summits);
            return (summits.Count, rating);
        }

        private static int TraceRating(IntMap map, Point p, int level, HashSet<Point> summits)
        {
            if (p.X < 0 || p.Y < 0 || p.X >= map.Width || p.Y >= map.Height) return 0;
            int here = map[p];
            if (here != level) return 0;
            if (here == 9)
            {
                summits.Add(p);
                return 1;
            }

            return TraceRating(map, p + Up, level + 1, summits)
                 + TraceRating(map, p + Right, level + 1, summits)
                 + TraceRating(map, p + Down, level + 1, summits)
                 + TraceRating(map, p + Left, level + 1, summits);
        }

    }
}
