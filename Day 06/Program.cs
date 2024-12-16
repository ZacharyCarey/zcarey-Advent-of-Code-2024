using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using Day_04;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Day_06
{
    internal class Program : ProgramStructure<Day6Input>
    {

        Program() : base(input => new Day6Input(input))
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(Day6Input input)
        {
            HashSet<Point> cells = [..input.Guard.Simulate(input).Select(x => x.Pos)];
            foreach(Guard sim in input.Guard.Simulate(input))
            {
                cells.Add(sim.Pos);
            }

            return cells.Count;
        }

        protected override object SolvePart2(Day6Input input)
        {
            HashSet<Point> cells = [.. input.Guard.Simulate(input).Select(x => x.Pos)];
            foreach (Guard sim in input.Guard.Simulate(input))
            {
                cells.Add(sim.Pos);
            }

            //input.Print();

            long obstructionCount = 0;
            foreach(var cell in cells) {
                if (cell == input.Start) continue;
                if (input[cell] != '.') continue;

                input[cell] = 'O'; // Create the temporary obstruction
                if (CheckForLoop(input, cell))
                {
                    obstructionCount++;
                }
                input[cell] = '.'; // Remove the temporary obstruction
            };

            return obstructionCount;
        }

        private static bool CheckForLoop(Day6Input input, Point p)
        {
            int[,] directionFlags = new int[input.Width, input.Height];

            //Console.WriteLine(p);
            //input.Print();

            // Look for a loop
            foreach(Guard sim in input.Guard.Simulate(input))
            {
                int dirFlag = (1 << sim.DirIndex);
                if ((directionFlags[sim.Pos.X, sim.Pos.Y] & dirFlag) == dirFlag)
                {
                    // The guard has been on this cell facing this direction before
                    return true;
                }
                directionFlags[sim.Pos.X, sim.Pos.Y] |= dirFlag;
            }

            return false;
        }
    }

    public struct Guard
    {
        public static Size[] Dirs = { new Size(0, -1), new Size(1, 0), new Size(0, 1), new Size(-1, 0) };
        public int DirIndex = 0;
        public Size Dir => Dirs[DirIndex];

        public Point Pos;

        public Guard() { }

        public IEnumerable<Guard> Simulate(Day6Input map)
        {
            while (IsOnMap(map))
            {
                yield return this;
                char c = LookForward(map);
                if (c == '#' || c == 'O')
                {
                    TurnRight();
                }
                else
                {
                    Walk();
                }
            }
        }

        public char LookForward(Day6Input map)
        {
            Point p = Pos + Dir;
            if (p.X < 0 || p.X >= map.Width || p.Y < 0 || p.Y >= map.Height)
            {
                return '.';
            }
            else
            {
                return map[Pos + Dir];
            }
        }

        public void Walk()
        {
            this.Pos += Dir;
        }

        public void TurnRight()
        {
            this.DirIndex = (this.DirIndex + 1) % Dirs.Length;
        }

        public bool IsOnMap(Day6Input map)
        {
            return this.Pos.X >= 0 && this.Pos.X < map.Width
                && this.Pos.Y >= 0 && this.Pos.Y < map.Height;
        }
    }

    public class Day6Input : CharMap
    {
        public Guard Guard;
        public Point Start;

        public Day6Input(string input) : base(input.GetLines().ToArray())
        {
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    char c = this[x, y];
                    if (c == '^')
                    {
                        Guard.Pos = new Point(x, y);
                        Start = new Point(x, y);
                        return;
                    }
                }
            }
        }

    }
}
