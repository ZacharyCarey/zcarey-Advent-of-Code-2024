using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using Day_04;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace Day_15
{
    internal class Program : ProgramStructure<Warehouse>
    {

        Program() : base(Warehouse.Parse)
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(Warehouse input)
        {
            input.Simulate(input.Directions);

            return input.AllPoints.Where(x => x.Value == 'O').Select(x => x.Position).Select(GPS).Sum();
        }

        protected override object SolvePart2(Warehouse input)
        {
            WideWarehouse warehouse = new WideWarehouse(input);
            warehouse.Simulate(input.Directions);

            return warehouse.AllPoints.Where(x => x.Value == '[').Select(x => x.Position).Select(GPS).Sum();
        }

        static long GPS(int x, int y)
        {
            return 100 * y + x;
        }

        static long GPS(Point p)
        {
            return GPS(p.X, p.Y);
        }
    }

    internal class Warehouse : CharMap
    {
        public List<Direction> Directions;
        Point Robot;

        public Warehouse(string[] data, List<Direction> directions) : base(data)
        {
            this.Directions = directions;

            for(int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    if (this[x, y] == '@')
                    {
                        this.Robot = new Point(x, y);
                        return;
                    }
                }
            }
        }

        public static Warehouse Parse(string input)
        {
            var blocks = input.GetBlocks().ToList();
            IEnumerable<Direction> directions = blocks[1].SelectMany(x => x.AsEnumerable()).Select(c => (Direction)(int)c);
            var result = new Warehouse(blocks[0].ToArray(), directions.ToList());
            return result;
        }

        public void Simulate(Direction direction)
        {
            Size offset = GetDirection(direction);
            char look = this[this.Robot + offset];
            if (look == '.')
            {
                this[Robot] = '.';
                Robot += offset;
                this[Robot] = '@';
            } else if (look == '#')
            {
                // Do nothing, can't move into a wall
            } else if (look == 'O')
            {
                // Find where the last box will get pushed to
                Point finalBoxPush = Robot + offset;
                while (this[finalBoxPush] == 'O')
                {
                    finalBoxPush += offset;
                }

                char finalBox = this[finalBoxPush];
                if (finalBox == '.')
                {
                    // Box gets pushed to an empty square, execute the move
                    this[Robot] = '.';
                    Robot += offset;
                    this[Robot] = '@';
                    this[finalBoxPush] = 'O';
                } else if (finalBox == '#')
                {
                    // Do nothing, cant push boxes into the wall
                } else
                {
                    throw new Exception("Cant push box into unknown item on map.");
                }

            } else
            {
                throw new Exception("Unknown item on map.");
            }
        }

        public void Simulate(IEnumerable<Direction> directions)
        {
            foreach(var direction in directions)
            {
                this.Simulate(direction);
            }
        }

        private static Size GetDirection(Direction dir)
        {
            switch (dir)
            {
                case Direction.Left: return new Size(-1, 0);
                case Direction.Right: return new Size(1, 0);
                case Direction.Up: return new Size(0, -1);
                case Direction.Down: return new Size(0, 1);
                default: throw new Exception("Unknown direction");
            }
        }
    }

    internal enum Direction : int
    {
        Up = '^',
        Down = 'v',
        Left = '<',
        Right = '>'
    }

    internal class WideWarehouse : CharMap
    {
        public List<Direction> Directions;
        Point Robot;

        public WideWarehouse(Warehouse input) : base(input.AsEnumerable().Select(WidenMap).ToArray())
        {
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    if (this[x, y] == '@')
                    {
                        this.Robot = new Point(x, y);
                    }
                }
            }
        }

        private static string WidenMap(IEnumerable<char> input)
        {
            StringBuilder str = new();
            foreach(var c in input)
            {
                switch (c)
                {
                    case 'O':
                        str.Append("[]");
                        break;
                    case '@':
                        str.Append("@.");
                        break;
                    case '#':
                    case '.':
                        str.Append(c, 2);
                        break;
                    default: throw new Exception("Unknown map icon.");
                }
            }

            return str.ToString();
        }

        public void Simulate(Direction direction)
        {
            Size offset = GetDirection(direction);
            char look = this[this.Robot + offset];
            if (look == '.')
            {
                this[Robot] = '.';
                Robot += offset;
                this[Robot] = '@';
            }
            else if (look == '#')
            {
                // Do nothing, can't move into a wall
            }
            else if (IsBox(look))
            {
                // Check if the box is pushable
                if (CheckIfPushable(this.Robot + offset, offset))
                {
                    // If it is, push the box and move the robot
                    Push(this.Robot + offset, offset);
                    this[Robot] = '.';
                    Robot += offset;
                    this[Robot] = '@';
                }
            }
            else
            {
                throw new Exception("Unknown item on map.");
            }
        }

        public void Simulate(IEnumerable<Direction> directions)
        {
            foreach (var direction in directions)
            {
                this.Simulate(direction);
            }
        }

        private static Size GetDirection(Direction dir)
        {
            switch (dir)
            {
                case Direction.Left: return new Size(-1, 0);
                case Direction.Right: return new Size(1, 0);
                case Direction.Up: return new Size(0, -1);
                case Direction.Down: return new Size(0, 1);
                default: throw new Exception("Unknown direction");
            }
        }

        private static bool IsBox(char c)
        {
            return c == '[' || c == ']';
        }

        private void Push(Point p, Size dir)
        {
            // If calling this we confirmed using CheckIfPushable() that these blocks should be moveable
            if (this[p] == ']') p.X--;
            if (dir.Height == 0)
            {
                // Pushing left/right
                Point b = p;
                if (dir.Width < 0)
                {
                    b.X--;
                }
                else
                {
                    b.X += 2;
                }

                // Push other box if needed
                char c = this[b];
                if (IsBox(c))
                {
                    Push(b, dir);
                }

                // Move this box
                this[p] = '.';
                this[p.X + 1, p.Y] = '.';
                p += dir;
                this[p] = '[';
                this[p.X + 1, p.Y] = ']';
            }
            else
            {
                // Pushing up/ down

                // Check left space and push box if needed
                char cLeft = this[p + dir];
                if (IsBox(cLeft))
                {
                    Push(p + dir, dir);
                }

                // Check right space and push box if needed
                Point pR = p + dir + new Size(1, 0);
                char cRight = this[pR];
                if (IsBox(cRight))
                {
                    Push(pR, dir);
                }

                // Move this box
                this[p] = '.';
                this[p.X + 1, p.Y] = '.';
                p += dir;
                this[p] = '[';
                this[p.X + 1, p.Y] = ']';
            }
        }

        private bool CheckIfPushable(Point p, Size dir)
        {
            if (this[p] == ']') p.X--;
            if (dir.Height == 0)
            {
                // Check pushing left/right
                if (dir.Width < 0)
                {
                    p.X--;
                } else
                {
                    p.X += 2;
                }
                char c = this[p];
                if (c == '.')
                {
                    return true;
                } else if (IsBox(c))
                {
                    return CheckIfPushable(p, dir);
                } else if (c == '#')
                {
                    return false;
                } else
                {
                    throw new Exception("Unknown map icon");
                }
            } else
            {
                // Pushing up/ down

                // Check left space
                char cLeft = this[p + dir];
                if (cLeft == '.')
                {
                    //leftPushable = true;
                } else if (IsBox(cLeft))
                {
                    if (!CheckIfPushable(p + dir, dir)) return false;
                } else if (cLeft == '#')
                {
                    return false;
                } else
                {
                    throw new Exception("Unknown map icon");
                }

                // Check right space
                Point pR = p + dir + new Size(1, 0);
                char cRight = this[pR];
                if (cRight == '.')
                {

                } else if (IsBox(cRight))
                {
                    if (!CheckIfPushable(pR, dir)) return false;
                } else if (cRight == '#')
                {
                    return false;
                } else
                {
                    throw new Exception("Unknown map icon");
                }

                return true;
            }
        }
    }
}
