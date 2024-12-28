using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using Day_04;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Day_16
{
    internal class Program : ProgramStructure<Maze>
    {
        Program() : base(Maze.Parse)
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(Maze input)
        {
            Func<Reindeer, bool> isEnd = (Reindeer node) =>
            {
                return node.Position == input.End;
            };

            Func<Reindeer, IEnumerable<(Reindeer, long)>> getNeighbors = (Reindeer node) =>
            {
                return GetNeighbors(input, node);
            };

            return Dijkstra.Search(input.Start, isEnd, getNeighbors).Select(x => x.Cost).First();
        }

        protected override object SolvePart2(Maze input)
        {
            Func<Reindeer, bool> isEnd = (Reindeer node) =>
            {
                return node.Position == input.End;
            };

            Func<Reindeer, IEnumerable<(Reindeer, long)>> getNeighbors = (Reindeer node) =>
            {
                return GetNeighbors(input, node);
            };

            var allPaths = Dijkstra.Search(input.Start, isEnd, getNeighbors);

            HashSet<Point> allSeats = new();
            foreach(var path in allPaths.Select(x => x.Path))
            {
                foreach(var seat in path.Select(x => x.Position))
                {
                    allSeats.Add(seat);
                }
            }

            return allSeats.Count;
        }

        private static IEnumerable<(Reindeer Edge, long Cost)> GetNeighbors(Maze maze, Reindeer state)
        {
            Reindeer copy = state;
            if (copy.LookForward(maze) != '#')
            {
                copy.MoveForward();
                yield return (copy, 1);
            }

            copy = state;
            copy.RotateClockwise();
            if (copy.LookForward(maze) != '#')
            {
                copy.MoveForward();
                yield return (copy, 1001);
            }

            copy = state;
            copy.RotateCounterClockwise();
            if (copy.LookForward(maze) != '#')
            {
                copy.MoveForward();
                yield return (copy, 1001);
            }
        }
    }

    internal struct Reindeer : IEqualityOperators<Reindeer, Reindeer, bool>, IEquatable<Reindeer>
    {
        public Point Position;
        public int X => Position.X;
        public int Y => Position.Y;

        public int DirectionIndex = 0;
        internal static Size[] Directions = [new Size(1, 0), new Size(0, 1), new Size(-1, 0), new Size(0, -1)]; // East, South, West, North
        public Size Direction => Directions[DirectionIndex];

        public Reindeer(Point p)
        {
            this.Position = p;
        }

        public Reindeer(Point p, int d)
        {
            this.Position = p;
            this.DirectionIndex = d;
        }

        public void MoveForward()
        {
            this.Position += Direction;
        }

        public char LookForward(Maze maze)
        {
            return maze[this.Position + Direction];
        }

        public void RotateClockwise()
        {
            DirectionIndex = (DirectionIndex + 1) % Directions.Length;
        }

        public void RotateCounterClockwise()
        {
            DirectionIndex--;
            if (DirectionIndex == -1) DirectionIndex = Directions.Length - 1;
        }

        public static bool operator ==(Reindeer left, Reindeer right)
        {
            return (left.Position == right.Position) && (left.DirectionIndex == right.DirectionIndex); 
        }

        public static bool operator !=(Reindeer left, Reindeer right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Position.GetHashCode(), this.DirectionIndex.GetHashCode());
        }

        public bool Equals(Reindeer other)
        {
            return this == other;
        }
    }

    internal class Maze : CharMap
    {
        public Reindeer Start;
        public Point End;

        public Maze(string[] data) : base(data)
        {
            foreach((Point Position, char Value) in this.AllPoints)
            {
                if (Value == 'S')
                {
                    Start = new Reindeer(Position);
                } else if (Value == 'E')
                {
                    End = Position;
                }
            }
        }

        public static Maze Parse(string input)
        {
            return new Maze(input.GetLines().ToArray());
        }
    }
}
