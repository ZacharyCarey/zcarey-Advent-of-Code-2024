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
        private long part1 = -1;
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
            PriorityQueue<(long cost, Reindeer state), long> nodesToProcess = new();
            Dictionary<Reindeer, long> seen = new();
            seen[input.Start] = 0;
            nodesToProcess.Enqueue((0, input.Start), 0);

            while (nodesToProcess.Count > 0)
            {
                (long curCost, Reindeer curState) = nodesToProcess.Dequeue();
                if (curState.Position == input.End) return curCost;
                if (seen.GetValueOrDefault(curState, long.MaxValue) < curCost) continue;
                seen[curState] = curCost;

                foreach(var neighbor in GetNeighbors(input, curState))
                {
                    long newCost = curCost + neighbor.Cost;
                    nodesToProcess.Enqueue((newCost, neighbor.Edge), newCost);
                }
            }

            return -1;
        }

        protected override object SolvePart2(Maze input)
        {
            PriorityQueue<(long cost, Reindeer state, HashSet<Point> visited), long> nodesToProcess = new();
            Dictionary<Reindeer, long> seen = new();
            HashSet<Point> bestSeats = new();
            bestSeats.Add(input.Start.Position);
            seen[input.Start] = 0;
            nodesToProcess.Enqueue((0, input.Start, new()), 0);
            long bestScore = long.MaxValue;
            while (nodesToProcess.Count > 0)
            {
                (long curCost, Reindeer curState, var visited) = nodesToProcess.Dequeue();
                if (curCost > bestScore) break;
                if (curState.Position == input.End)
                {
                    bestScore = curCost;
                    bestSeats.UnionWith(visited);
                }
                if (seen.GetValueOrDefault(curState, long.MaxValue) < curCost) continue;
                seen[curState] = curCost;

                foreach(var neighbor in GetNeighbors(input, curState))
                {
                    HashSet<Point> seenLocs = new(visited);
                    seenLocs.Add(neighbor.Edge.Position);
                    long newCost = curCost + neighbor.Cost;
                    nodesToProcess.Enqueue((newCost, neighbor.Edge, seenLocs), newCost);
                }
            }

            return bestSeats.Count;
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

    internal struct Reindeer : IEqualityOperators<Reindeer, Reindeer, bool>
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
