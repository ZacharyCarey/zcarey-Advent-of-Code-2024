using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Day_18
{
    internal class Program : ProgramStructure<Point[]>
    {

        Program() : base(x => x.GetLines().Select(x => x.Split(',').Select(int.Parse).ToArray()).Select(x => new Point(x[0], x[1])).ToArray())
        { }

        //static int Size = 7;
        static int Size = 71;

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(Point[] input)
        {
            bool[,] memory = new bool[Size,Size];
            for(int i = 0; i < 1024; i++)
            {
                Point bite = input[i];
                memory[bite.X, bite.Y] = true;
            }

            Func<Point, IEnumerable<(Point Node, long Cost)>> getNeighbors = p => GetNeighbors(memory, p);
            (long cost, List<Point> path) = Dijkstra.Search(new Point(), new Point(70, 70), getNeighbors).First();
            return path.Count - 1;
        }

        private static IEnumerable<(Point Node, long Cost)> GetNeighbors(bool[,] memory, Point loc)
        {
            if (loc.X > 0 && memory[loc.X - 1, loc.Y] == false) yield return (new Point(loc.X - 1, loc.Y), 1);
            if (loc.Y > 0 && memory[loc.X, loc.Y - 1] == false) yield return (new Point(loc.X, loc.Y - 1), 1);
            if (loc.X < Size - 1 && memory[loc.X + 1, loc.Y] == false) yield return (new Point(loc.X + 1, loc.Y), 1);
            if (loc.Y < Size - 1 && memory[loc.X, loc.Y + 1] == false) yield return (new Point(loc.X, loc.Y + 1), 1);
        }

        protected override object SolvePart2(Point[] input)
        {
            bool[,] memory = new bool[Size, Size];
            // 1024 bytes is guranteed to have a path
            for (int i = 0; i < 1024; i++)
            {
                Point bite = input[i];
                memory[bite.X, bite.Y] = true;
            }

            // Get any path to the exit
            Func<Point, IEnumerable<(Point Node, long Cost)>> getNeighbors = p => GetNeighbors(memory, p);
            (long _, List<Point> path) = Dijkstra.Search(new Point(), new Point(70, 70), getNeighbors).First();

            // Now try and find the byte that blocks the exit
            for (int i = 1024; i < input.Length; i++)
            {
                Point bite = input[i];
                memory[bite.X, bite.Y] = true;
                if (path.Contains(bite))
                {
                    // This byte blocks the current calculated exit route, so we have to recalculate a new route.
                    var route = Dijkstra.Search(new Point(), new Point(70, 70), getNeighbors).Take(1).ToArray();
                    if (route.Length == 0)
                    {
                        // No more valid exit paths!
                        return $"{bite.X},{bite.Y}";
                    }
                    path = route[0].Path;
                }
            }

            return "Failed to find answer.";
        }

    }
}
