using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Day_21
{
    internal class Program : ProgramStructure<IEnumerable<DoorCode>>
    {

        Program() : base(x => x.GetLines().Select(DoorCode.Parse))
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(IEnumerable<DoorCode> input)
        {
            NumericalKeypad keypad = new(
                new DirectionalKeypad(
                    new DirectionalKeypad(
                        new HumanKeypad()
                    )
                )
            );

            long complexity = 0;
            foreach(var code in input)
            {
                long presses = code.Digits.Append(NumericalKeypad.A).Select(keypad.Press).Sum();
                complexity += presses * code.Number;
            }

            return complexity;
        }

        protected override object SolvePart2(IEnumerable<DoorCode> input)
        {
            Keypad nestedKeypad = new HumanKeypad();
            for (int i = 0; i < 25; i++)
            {
                nestedKeypad = new DirectionalKeypad(nestedKeypad);
            }
            NumericalKeypad keypad = new(nestedKeypad);

            long complexity = 0;
            foreach(var code in input)
            {
                long preses = code.Digits.Append(NumericalKeypad.A).Select(keypad.Press).Sum();
                complexity += preses * code.Number;
            }

            return complexity;
        }

    }

    internal struct DoorCode
    {
        public int Number;
        public int[] Digits;

        public DoorCode(string input)
        {
            string str = input[..^1];
            Number = int.Parse(str);
            Digits = str.Select(c => c - '0').ToArray();
        }
        
        public static DoorCode Parse(string input)
        {
            return new DoorCode(input);
        }
    }

    internal abstract class Keypad
    {
        public abstract long Press(IEnumerable<Size> directions);

        public static Size GetDelta(Point position, Point target)
        {
            return new Size(
                target.X - position.X,
                target.Y - position.Y
            );
        }

        public static IEnumerable<IEnumerable<Size>> GetPaths(Point position, Point target, Point ignore)
        {
            Size dist = Keypad.GetDelta(position, target);
            int w = Math.Abs(dist.Width);
            int h = Math.Abs(dist.Height);
            List<Size> path = new List<Size>(w + h);
            return TracePath(position, target, ignore, path);
        }

        private static IEnumerable<List<Size>> TracePath(Point position, Point target, Point ignore, List<Size> path)
        {
            if (position == ignore)
            {
                yield break;
            }

            if (position == target)
            {
                yield return path;
                yield break;
            }

            int index;
            if (position.X != target.X)
            {
                // Move left/right
                int delta = (position.X > target.X) ? -1 : 1;
                index = path.Count;
                path.Add(new Size(delta, 0));
                foreach (var paths in TracePath(new Point(position.X + delta, position.Y), target, ignore, path)) yield return paths;
                path.RemoveAt(index);
            }
            if (position.Y != target.Y)
            {
                // Move up/down
                int delta = (position.Y > target.Y) ? -1 : 1;
                index = path.Count;
                path.Add(new Size(0, delta));
                foreach (var paths in TracePath(new Point(position.X, position.Y + delta), target, ignore, path)) yield return paths;
                path.RemoveAt(index);
            }
        }
    }

    internal class NumericalKeypad
    {
        private Keypad Controller;
        private Point Position = ButtonLocations[NumericalKeypad.A];
        private static Point[] ButtonLocations = [
            new Point(1, 3),
            new Point(0, 2),
            new Point(1, 2),
            new Point(2, 2),
            new Point(0, 1),
            new Point(1, 1),
            new Point(2, 1),
            new Point(0, 0),
            new Point(1, 0),
            new Point(2, 0),
            new Point(2, 3)
        ];
        public const int A = 10;
        private static Point Hole = new Point(0, 3);

        public NumericalKeypad(Keypad controller)
        {
            this.Controller = controller;
        }

        public long Press(int button)
        {
            Point target = ButtonLocations[button];

            long lowestPresses = long.MaxValue;
            foreach(var path in Keypad.GetPaths(this.Position, target, Hole))
            {
                long presses = this.Controller.Press(path);
                if (presses < lowestPresses)
                {
                    lowestPresses = presses;
                }
            }

            Position = target;

            return lowestPresses;
        }
    }

    internal class DirectionalKeypad : Keypad
    {
        protected Point Position = DirectionalKeypad.A;
        private Keypad Controller;

        public static Point A = new Point(2, 0);
        public static Point Up = new Point(1, 0);
        public static Point Down = new Point(1, 1);
        public static Point Left = new Point(0, 1);
        public static Point Right = new Point(2, 1);
        public static Point Hole = new Point(0, 0);

        private Dictionary<(Point, Point), long> presses = new();

        public DirectionalKeypad(Keypad controller)
        {
            this.Controller = controller;
        }

        private Point GetButtonPosition(Size dir)
        {
            if (dir.Width == -1) return Left;
            else if (dir.Width == 1) return Right;
            else if (dir.Height == -1) return Up;
            else if (dir.Height == 1) return Down;
            else if (dir.Height == 0 && dir.Width == 0) return A;
            throw new Exception("Unknown direction.");
        }

        public override long Press(IEnumerable<Size> directions)
        {
            long totalPresses = 0;
            foreach(Size direction in directions.Append(new Size(0, 0)))
            {
                Point target = GetButtonPosition(direction);

                long lowestPresses;
                if (!presses.TryGetValue((Position, target), out lowestPresses))
                {
                    lowestPresses = long.MaxValue;
                    foreach (var path in Keypad.GetPaths(this.Position, target, Hole))
                    {
                        long presses = this.Controller.Press(path);
                        if (presses < lowestPresses)
                        {
                            lowestPresses = presses;
                        }
                    }
                    presses[(Position, target)] = lowestPresses;
                }

                Position = target;
                totalPresses += lowestPresses;
            }

            return totalPresses;
        }
    }

    internal class HumanKeypad : Keypad
    {
        public override long Press(IEnumerable<Size> directions)
        {
            return directions.Count() + 1;
        }
    }
}
