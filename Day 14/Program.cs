using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day_14
{
    internal class Program : ProgramStructure<List<Robot>>
    {

        Program() : base(x => x.GetLines().Create<string, Robot>().ToList())
        { }

        //Size MapSize = new Size(11, 7); // Example
        Size MapSize = new Size(101, 103);

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(List<Robot> input)
        {
            int[] quadCount = new int[5];
            foreach(var robot in input)
            {
                robot.Simulate(100, MapSize);
                quadCount[robot.Quadrant(MapSize)]++;
            }

            return quadCount[1] * quadCount[2] * quadCount[3] * quadCount[4];
        }

        protected override object SolvePart2(List<Robot> input)
        {
            char[] map = new char[MapSize.Width * MapSize.Height];

            Console.WriteLine("Press enter to locate a possible tree");
            int secs = 0;
            while(true)
            {
                Console.ReadLine();

                // Find the next symmetrical pattern by checking for situations where over half of the robots
                // are in the middle 1/3 of the map
                int left = MapSize.Width / 3;
                int right = MapSize.Width - (MapSize.Width / 3);
                int robotsInMiddle = 0;
                while (robotsInMiddle < input.Count / 2)
                {
                    secs++;
                    robotsInMiddle = 0;
                    foreach (var robot in input)
                    {
                        robot.Simulate(1, MapSize);
                        if (robot.Position.X >= left && robot.Position.X <= right)
                        {
                            robotsInMiddle++;
                        }
                    }
                }
                
                // Print the result
                Console.WriteLine();
                Console.WriteLine($"Elapsed Time: {secs}s");
                Array.Fill(map, ' ');
                foreach (var robot in input)
                {
                    map[MapSize.Width * robot.Position.Y + robot.Position.X] = '#';
                }
                for (int y = 0; y < MapSize.Height; y++)
                {
                    Console.WriteLine(new string(map.AsSpan(MapSize.Width * y, MapSize.Width)));
                } 
            }
            return null;
        }
    }

    internal class Robot : IObjectParser<string, Robot>
    {
        private static readonly Regex RobotRegex = new(@"p=(\d+),(\d+) v=(-?\d+),(-?\d+)", RegexOptions.Compiled);

        public Point Position;
        public Size Velocity;

        public Robot(Point position, Size velocity)
        {
            this.Position = position;
            this.Velocity = velocity;
        }

        public static Robot Parse(string input)
        {
            var match = RobotRegex.Match(input);
            Point pos = new Point(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
            Size vel = new Size(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));
            return new Robot(pos, vel);
        }

        public void Simulate(int seconds, Size mapSize)
        {
            this.Position += this.Velocity * seconds;
            this.Position.X = mod(this.Position.X, mapSize.Width);
            this.Position.Y = mod(this.Position.Y, mapSize.Height);
        }

        public int Quadrant(Size mapSize)
        {
            int lowerW = (mapSize.Width / 2) - 1;
            int upperW = mapSize.Width - (mapSize.Width / 2);

            int lowerH = (mapSize.Height / 2) - 1;
            int upperH = mapSize.Height - (mapSize.Height / 2);

            if (this.Position.X <= lowerW)
            {
                if (this.Position.Y <= lowerH)
                {
                    return 1;
                }
                else if (this.Position.Y >= upperH)
                {
                    return 3;
                }
            } else if (this.Position.X >= upperW)
            {
                if (this.Position.Y <= lowerH)
                {
                    return 2;
                } else if (this.Position.Y >= upperH)
                {
                    return 4;
                }
            }

            return 0;
        }

        // Proper modulus that will wrap around the negative numbers
        static int mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
    }
}
