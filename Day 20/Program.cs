using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using Day_04;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Day_20
{
    internal class Program : ProgramStructure<Day20Input>
    {

        Program() : base(Day20Input.Parse)
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(Day20Input input)
        {
            IEnumerable<List<Point>> paths = Dijkstra.Search(input.Map, input.Start, input.End, out long? lowestCostPath, out long[,] costMap);

            long decentCheats = 0;
            HashSet<Point> path = new HashSet<Point>(paths.First());
            foreach (Point point in path)
            {
                long startDistance = costMap[point.X, point.Y];
                foreach(Point ring in GetRing(point, 2, input.Width, input.Height).Where(x => input.Map[x.X, x.Y] == true))
                {
                    long newDist = costMap[ring.X, ring.Y];
                    long timeSaved = (newDist - startDistance) - 2;
                    if (timeSaved >= 100)
                    {
                        decentCheats++;
                    }
                }
            }

            return decentCheats;
        }

        private static Size[] Ring = [new Size(1, 1), new Size(-1, 1), new Size(-1, -1), new Size(1, -1)];

        private static IEnumerable<Point> GetRing(Point center, int radius, int width, int height)
        {
            Point p = center + new Size(0, -radius);
            for (int i = 0; i < Ring.Length; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    p += Ring[i];
                    if (p.X >= 0 && p.X < width && p.Y >= 0 && p.Y < height)
                    {
                        yield return p;
                    }
                }
            }
        }

        protected override object SolvePart2(Day20Input input)
        {
            IEnumerable<List<Point>> paths = Dijkstra.Search(input.Map, input.Start, input.End, out long? lowestCostPath, out long[,] costMap);

            long decentCheats = 0;
            HashSet<Point> path = new HashSet<Point>(paths.First());
            foreach (Point point in path)
            {
                long startDistance = costMap[point.X, point.Y];
                foreach ((Point ring, int dist) in GetFilledRing(point, 20, input.Width, input.Height).Where(x => input.Map[x.p.X, x.p.Y] == true))
                {
                    long newDist = costMap[ring.X, ring.Y];
                    long timeSaved = (newDist - startDistance) - dist;
                    if (timeSaved >= 100)
                    {
                        decentCheats++;
                    }
                }
            }

            return decentCheats;
        }


        private static IEnumerable<(Point p, int dist)> GetFilledRing(Point center, int radius, int width, int height)
        {
            Func<Point, bool> isInMap = p =>
            {
                return p.X >= 0 && p.X < width && p.Y >= 0 && p.Y < height;
            };

            for (int x = -radius, i = 0; x <= 0; x++, i++)
            {
                for (int y = -i; y <= i; y++)
                {
                    Point p = new Point(center.X + x, center.Y + y);
                    if (isInMap(p))
                    {
                        yield return (p, Math.Abs(x) + Math.Abs(y));
                    }
                }
            }

            for (int x = radius, i = 0; x > 0; x--, i++)
            {
                for (int y = -i; y <= i; y++)
                {
                    Point p = new Point(center.X + x, center.Y + y);
                    if (isInMap(p))
                    {
                        yield return (p, Math.Abs(x) + Math.Abs(y));
                    }
                }
            }
        }
    }

    internal class Day20Input
    {
        public int Width;
        public int Height;
        public bool[,] Map;

        public Point Start;
        public Point End;

        public Day20Input(string input)
        {
            CharMap map = CharMap.Parse(input);
            Width = map.Width;
            Height = map.Height;
            Map = new bool[map.Width, map.Height];
            foreach(var cell in map.AllPoints)
            {
                if (cell.Value == 'S')
                {
                    Start = cell.Position;
                } else if (cell.Value == 'E')
                {
                    End = cell.Position;
                }

                Map[cell.Position.X, cell.Position.Y] = (cell.Value != '#');
            }
        }

        public static Day20Input Parse(string input)
        {
            return new Day20Input(input);
        }
    }
}
