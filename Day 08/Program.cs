using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using Day_04;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Day_08
{
    internal class Program : ProgramStructure<CharMap>
    {

        Program() : base(CharMap.Parse)
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(CharMap input)
        {
            Dictionary<char, List<Point>> Nodes = new();
            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    char c = input[x, y];
                    if (c != '.')
                    {
                        List<Point> nodes;
                        if (!Nodes.TryGetValue(c, out nodes))
                        {
                            nodes = new List<Point>();
                            Nodes[c] = nodes;
                        }
                        nodes.Add(new Point(x, y));
                    }
                }
            }

            HashSet<Point> UniqueAntinodes = new();
            foreach ((char freq, List<Point> nodes) in Nodes)
            {
                // Check every pair of nodes on the same frequency
                for (int i = 0; i < nodes.Count - 1; i++)
                {
                    for (int j = i + 1; j < nodes.Count; j++)
                    {
                        foreach(Point antinode in FindAntinodes(input, nodes[i], nodes[j]))
                        {
                            UniqueAntinodes.Add(antinode);
                        }
                    }
                }
            }

            return UniqueAntinodes.Count;
        }

        private static IEnumerable<Point> FindAntinodes(CharMap input, Point a, Point b, bool resonate = false)
        {
            Size delta = new (a.X - b.X, a.Y - b.Y);

            if (!resonate)
            {
                Point p1 = b - delta;
                Point p2 = a + delta;

                if (InMap(input, p1)) yield return p1;
                if (InMap(input, p2)) yield return p2;
            }else
            {
                for (Point p = b; InMap(input, p); p -= delta)
                {
                    yield return p;
                }

                for (Point p = a; InMap(input, p); p += delta)
                {
                    yield return p;
                }
            }
        }

        private static bool InMap(CharMap input, Point p)
        {
            return p.X >= 0 && p.Y >= 0 && p.X < input.Width && p.Y < input.Height;
        }

        protected override object SolvePart2(CharMap input)
        {
            Dictionary<char, List<Point>> Nodes = new();
            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    char c = input[x, y];
                    if (c != '.')
                    {
                        List<Point> nodes;
                        if (!Nodes.TryGetValue(c, out nodes))
                        {
                            nodes = new List<Point>();
                            Nodes[c] = nodes;
                        }
                        nodes.Add(new Point(x, y));
                    }
                }
            }

            HashSet<Point> UniqueAntinodes = new();
            foreach ((char freq, List<Point> nodes) in Nodes)
            {
                // Check every pair of nodes on the same frequency
                for (int i = 0; i < nodes.Count - 1; i++)
                {
                    for (int j = i + 1; j < nodes.Count; j++)
                    {
                        foreach (Point antinode in FindAntinodes(input, nodes[i], nodes[j], true))
                        {
                            UniqueAntinodes.Add(antinode);
                        }
                    }
                }
            }

            return UniqueAntinodes.Count;
        }

    }
}
